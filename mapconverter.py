import os
import struct
import argparse
import colorama
from colorama import Fore, Back, Style

# Initialize colorama for Windows support
colorama.init()

def visualize_conversion_process(lines, width, height):
    """
    Visualizes the conversion process from text to binary
    """
    print(f"\n{Fore.CYAN}=== Original Text Map ({width}x{height}) ==={Style.RESET_ALL}")
    # Show the original map with colored tiles
    for line in lines:
        for char in line:
            if char == '1':
                print(f"{Back.WHITE}{Fore.BLACK}1{Style.RESET_ALL}", end='')
            else:
                print(f"{Back.BLACK}{Fore.WHITE}0{Style.RESET_ALL}", end='')
        print()
    
    print(f"\n{Fore.CYAN}=== Binary File Structure ==={Style.RESET_ALL}")
    # Show header bytes
    print(f"{Fore.GREEN}Header Bytes:{Style.RESET_ALL}")
    print(f"Width  (1 byte) : {width:08b} ({width})")
    print(f"Height (1 byte) : {height:08b} ({height})")
    
    print(f"\n{Fore.GREEN}Data Bytes (bit-packed):{Style.RESET_ALL}")
    bytes_data = []
    
    # Process and show each row
    for y, line in enumerate(lines):
        print(f"\nRow {y + 1}:")
        current_byte = 0
        bit_position = 0
        bits_display = ""
        
        for x, char in enumerate(line):
            # Add visual separator every 8 bits
            if x > 0 and x % 8 == 0:
                print(f"Byte: {bits_display} = {current_byte:08b} ({current_byte})")
                bytes_data.append(current_byte)
                current_byte = 0
                bit_position = 0
                bits_display = ""
            
            # Show bit being processed
            if char == '1':
                bits_display += f"{Fore.RED}1{Style.RESET_ALL}"
                current_byte |= (1 << bit_position)
            else:
                bits_display += f"{Fore.BLUE}0{Style.RESET_ALL}"
            
            bit_position += 1
        
        # Show remaining bits in the last byte
        if bits_display:
            print(f"Byte: {bits_display} = {current_byte:08b} ({current_byte})")
            bytes_data.append(current_byte)
    
    return bytes_data

def convert_map_to_binary(input_path, output_path, visualize=False):
    """
    Convert a text-based collision map to binary format.
    Format:
    - First 2 bytes: width (1 byte) and height (1 byte) as unsigned chars
    - Remaining bytes: bit-packed collision data
    """
    try:
        # Read the text file
        with open(input_path, 'r') as f:
            lines = [line.strip() for line in f.readlines() if line.strip()]
        
        # Validate input
        if not lines:
            raise ValueError("Empty input file")
        
        height = len(lines)
        width = len(lines[0])
        
        # Check dimension limits
        if width > 255 or height > 255:
            raise ValueError(f"Map dimensions ({width}x{height}) exceed 255x255 limit")
        
        # Verify all lines have the same width
        if not all(len(line) == width for line in lines):
            raise ValueError("Inconsistent line lengths in input file")
        
        # Verify only 0s and 1s in the file
        if not all(all(c in '01' for c in line) for line in lines):
            raise ValueError("Input file contains characters other than 0 and 1")

        if visualize:
            bytes_data = visualize_conversion_process(lines, width, height)
        
        with open(output_path, 'wb') as f:
            # Write dimensions as single bytes
            f.write(bytes([width, height]))
            
            if not visualize:
                # Process each row without visualization
                for y in range(height):
                    current_byte = 0
                    bit_position = 0
                    
                    for x in range(width):
                        if lines[y][x] == '1':
                            current_byte |= (1 << bit_position)
                        
                        bit_position += 1
                        
                        if bit_position == 8 or x == width - 1:
                            f.write(bytes([current_byte]))
                            current_byte = 0
                            bit_position = 0
            else:
                # Write the data collected during visualization
                f.write(bytes(bytes_data))
        
        # Calculate and show file statistics
        original_size = os.path.getsize(input_path)
        binary_size = os.path.getsize(output_path)
        compression_ratio = (1 - binary_size / original_size) * 100
        
        if visualize:
            print(f"\n{Fore.CYAN}=== File Size ==={Style.RESET_ALL}")
            print(f"Header size: 2 bytes")
            bytes_per_row = (width + 7) // 8
            print(f"Data size  : {bytes_per_row * height} bytes ({bytes_per_row} bytes/row × {height} rows)")
            print(f"Total size : {binary_size} bytes")
            print(f"original size: {original_size} bytes")
            print(f"Compression ratio: {compression_ratio:.1f}%")
        else:
            print(f"Converted {input_path}")
            print(f"Map dimensions: {width}x{height}")
            print(f"Original size: {original_size} bytes")
            print(f"Binary size: {binary_size} bytes")
            print(f"Compression ratio: {compression_ratio:.1f}%")
        
    except Exception as e:
        print(f"Error converting {input_path}: {str(e)}")
        return False
    
    return True

def verify_binary_map(binary_path):
    """
    Verify the binary map file by reading it back and checking the format
    """
    try:
        with open(binary_path, 'rb') as f:
            header = f.read(2)
            width, height = header[0], header[1]
            
            expected_size = 2 + ((width + 7) // 8) * height
            actual_size = os.path.getsize(binary_path)
            
            if expected_size != actual_size:
                raise ValueError(
                    f"Invalid file size. Expected {expected_size}, got {actual_size}"
                )
            
            print(f"Verified {binary_path}: {width}x{height} map")
            return True
            
    except Exception as e:
        print(f"Error verifying {binary_path}: {str(e)}")
        return False

def main():
    parser = argparse.ArgumentParser(description='Convert collision map text files to binary format')
    parser.add_argument('input_files', nargs='+', help='Input text files to convert')
    parser.add_argument('--output-dir', '-o', default=None, help='Output directory for binary files')
    parser.add_argument('--visualize', '-v', action='store_true', help='Show visualization of conversion process')
    
    args = parser.parse_args()
    
    output_dir = args.output_dir or os.path.dirname(args.input_files[0])
    os.makedirs(output_dir, exist_ok=True)
    
    for input_file in args.input_files:
        if not input_file.endswith('.txt'):
            print(f"Skipping {input_file}: not a .txt file")
            continue
            
        output_file = os.path.join(
            output_dir,
            os.path.basename(input_file).replace('.txt', '.lcm')
        )
        
        print(f"\nProcessing: {input_file}")
        if convert_map_to_binary(input_file, output_file, args.visualize):
            verify_binary_map(output_file)

if __name__ == '__main__':
    main()