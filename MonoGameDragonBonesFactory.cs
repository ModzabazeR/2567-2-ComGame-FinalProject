using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DragonBones;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;

public class MonoGameDragonBonesFactory : BaseFactory
{
    private GraphicsDevice _graphicsDevice;
    private ContentManager _content;
    private new DragonBones.DragonBones _dragonBones;

    public MonoGameDragonBonesFactory(GraphicsDevice graphicsDevice, ContentManager content, DragonBones.DragonBones dragonBones)
    {
        _graphicsDevice = graphicsDevice;
        _content = content;
        _dragonBones = dragonBones;
        
    }

    public Armature BuildArmatureFromJson(string skeletonJsonPath, string atlasJsonPath, Texture2D atlasTexture, string armatureName = null)
    {
        try
        {
            // Add validation for the files
            if (!File.Exists(skeletonJsonPath)) {
                Console.WriteLine($"Skeleton file not found: {skeletonJsonPath}");
                return null;
            }
            if (!File.Exists(atlasJsonPath)) {
                Console.WriteLine($"Atlas file not found: {atlasJsonPath}");
                return null;
            }

            // Load skeleton data and validate it
            string skeletonJson = File.ReadAllText(skeletonJsonPath);
            string atlasJson = File.ReadAllText(atlasJsonPath);
            
            Console.WriteLine($"Skeleton JSON length: {skeletonJson.Length}");
            Console.WriteLine($"Atlas JSON length: {atlasJson.Length}");
            
            // Try pre-parsing the JSON to validate it
            var jObj = Newtonsoft.Json.JsonConvert.DeserializeObject(skeletonJson);
            
            // Continue with parsing...
            var dragonBonesData = JsonHelper.ParseDragonBonesData(skeletonJson);
            
            // Add the DragonBones data to the factory
            AddDragonBonesData(dragonBonesData);
            
            // Create a texture atlas
            var textureAtlasData = JsonHelper.ParseTextureAtlasData(atlasJson);
            
            // Create a MonoGame texture atlas
            var monoGameTextureAtlas = new MonoGameTextureAtlasData();
            monoGameTextureAtlas.name = textureAtlasData.name;
            monoGameTextureAtlas.texture = atlasTexture;
            
            // Access the texture data using the proper method
            foreach (var textureName in textureAtlasData.textures.Keys)
            {
                var textureData = textureAtlasData.GetTexture(textureName);
                
                var monoGameTextureData = monoGameTextureAtlas.CreateTexture() as MonoGameTextureData;
                monoGameTextureData.region.x = textureData.region.x;
                monoGameTextureData.region.y = textureData.region.y;
                monoGameTextureData.region.width = textureData.region.width;
                monoGameTextureData.region.height = textureData.region.height;
                monoGameTextureData.name = textureData.name;
                monoGameTextureData.rotated = textureData.rotated;
                monoGameTextureAtlas.AddTexture(monoGameTextureData);
            }
            
            // Add the texture atlas to the factory
            AddTextureAtlasData(monoGameTextureAtlas);
            
            // Build the armature
            string armatureToUse = armatureName ?? dragonBonesData.armatureNames[0];
            return BuildArmature(armatureToUse);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error building armature: {ex.Message}\n{ex.StackTrace}");
            return null;
        }
    }

    // Add helper method to load from Content pipeline
    public new Armature BuildArmature(string skeletonJsonPath, string atlasJsonPath, string texturePath, string armatureName = null)
    {
        // Load the texture
        var texture = _content.Load<Texture2D>(texturePath);
        
        // Build the armature
        return BuildArmatureFromJson(skeletonJsonPath, atlasJsonPath, texture, armatureName);
    }

    protected override Armature _BuildArmature(BuildArmaturePackage dataPackage)
    {
        try
        {
            Console.WriteLine($"Building armature: {dataPackage.armature.name}");
            
            var armature = BaseObject.BorrowObject<Armature>();
            
            // Get the texture from the appropriate atlas
            Texture2D texture = null;
            if (dataPackage.textureAtlasName != null)
            {
                var atlasData = this.GetTextureAtlasData(dataPackage.textureAtlasName);
                if (atlasData.Count > 0 && atlasData[0] is MonoGameTextureAtlasData monoGameAtlasData)
                {
                    texture = monoGameAtlasData.texture;
                    Console.WriteLine($"Found texture: {(texture != null ? $"{texture.Width}x{texture.Height}" : "NULL")}");
                }
            }
            
            var armatureDisplay = new MonoGameArmatureDisplay(armature, texture);
            
            Console.WriteLine("Initializing armature...");
            armature.Init(
                dataPackage.armature,
                armatureDisplay,
                null,
                this._dragonBones
            );
            
            return armature;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in _BuildArmature: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            return null;
        }
    }

    protected override Slot _BuildSlot(BuildArmaturePackage dataPackage, SlotData slotData, Armature armature)
    {
        var slot = BaseObject.BorrowObject<MonoGameSlot>();
        slot.Init(slotData, armature, new MonoGameSlotDisplay(), new MonoGameSlotDisplay());
        return slot;
    }

    protected override TextureAtlasData _BuildTextureAtlasData(TextureAtlasData textureAtlasData, object textureAtlas)
    {
        var texture = textureAtlas as Texture2D;
        if (textureAtlasData != null && texture != null)
        {
            var monoGameTextureAtlasData = textureAtlasData as MonoGameTextureAtlasData;
            if (monoGameTextureAtlasData != null)
            {
                monoGameTextureAtlasData.texture = texture;
            }
        }

        return textureAtlasData;
    }

    private bool ValidateDragonBonesJson(Dictionary<string, object> skeletonDict)
    {
        // Check for required top-level properties
        if (!skeletonDict.ContainsKey("armature"))
        {
            Console.WriteLine("Error: JSON missing 'armature' property");
            return false;
        }
        
        // Print existing keys for debugging
        Console.WriteLine("Top-level keys in skeleton JSON:");
        foreach (var key in skeletonDict.Keys)
        {
            Console.WriteLine($"- {key}");
        }
        
        return true;
    }

    private void DumpJsonStructure(object obj, int depth)
    {
        string indent = new string(' ', depth * 2);
        
        if (obj is Dictionary<string, object> dict)
        {
            Console.WriteLine($"{indent}Dictionary ({dict.Count} keys)");
            foreach (var kvp in dict.Take(Math.Min(dict.Count, 5))) // Limit to first 5 to avoid huge output
            {
                Console.WriteLine($"{indent}- {kvp.Key}:");
                DumpJsonStructure(kvp.Value, depth + 1);
            }
        }
        else if (obj is List<object> list)
        {
            Console.WriteLine($"{indent}List ({list.Count} items)");
            if (list.Count > 0)
            {
                Console.WriteLine($"{indent}First item:");
                DumpJsonStructure(list[0], depth + 1);
            }
        }
        else if (obj != null)
        {
            Console.WriteLine($"{indent}{obj.GetType().Name}: {obj}");
        }
        else
        {
            Console.WriteLine($"{indent}null");
        }
    }

    private object ConvertJsonValue(object value)
    {
        if (value == null)
            return null;
        
        // Convert JValue to primitive type
        if (value is Newtonsoft.Json.Linq.JValue jValue)
        {
            return jValue.Value;
        }
        
        // Convert JObject to Dictionary
        if (value is Newtonsoft.Json.Linq.JObject jObject)
        {
            var dict = new Dictionary<string, object>();
            foreach (var prop in jObject.Properties())
            {
                dict[prop.Name] = ConvertJsonValue(prop.Value);
            }
            return dict;
        }
        
        // Convert JArray to List
        if (value is Newtonsoft.Json.Linq.JArray jArray)
        {
            var list = new List<object>();
            foreach (var item in jArray)
            {
                list.Add(ConvertJsonValue(item));
            }
            return list;
        }
        
        // Already a regular C# type
        return value;
    }

    // New method to validate armature structure
    private void ValidateArmatureStructure(Dictionary<string, object> armature)
    {
        // Check for required properties
        CheckProperty(armature, "name");
        CheckProperty(armature, "aabb", (aabb) => {
            if (aabb is Dictionary<string, object> aabbDict)
            {
                CheckProperty(aabbDict, "x", (x) => Console.WriteLine($"  - x: {x} ({x.GetType().Name})"));
                CheckProperty(aabbDict, "y", (y) => Console.WriteLine($"  - y: {y} ({y.GetType().Name})"));
                CheckProperty(aabbDict, "width", (w) => Console.WriteLine($"  - width: {w} ({w.GetType().Name})"));
                CheckProperty(aabbDict, "height", (h) => Console.WriteLine($"  - height: {h} ({h.GetType().Name})"));
            }
        });
        CheckProperty(armature, "bone");
        CheckProperty(armature, "slot");
    }

    // Helper method to check if property exists and output its status
    private void CheckProperty(Dictionary<string, object> dict, string key, Action<object> additionalCheck = null)
    {
        if (dict.ContainsKey(key))
        {
            Console.WriteLine($"- {key}: Present");
            additionalCheck?.Invoke(dict[key]);
        }
        else
        {
            Console.WriteLine($"- {key}: MISSING");
        }
    }
}

// Helper class for parsing JSON
public static class JsonHelper
{
    public static DragonBonesData ParseDragonBonesData(string json)
    {
        try
        {
            // Directly use the parser
            ObjectDataParser parser = new ObjectDataParser();
            return parser.ParseDragonBonesData(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing DragonBones data: {ex.Message}");
            return null;
        }
    }
    
    public static TextureAtlasData ParseTextureAtlasData(string json)
    {
        try
        {
            // Convert the JSON string to a Dictionary
            var rawData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            
            // Create a texture atlas data instance
            var textureAtlasData = new MonoGameTextureAtlasData();
            
            // Parse it with a parser
            ObjectDataParser parser = new ObjectDataParser();
            parser.ParseTextureAtlasData(rawData, textureAtlasData);
            
            return textureAtlasData;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing texture atlas data: {ex.Message}");
            return null;
        }
    }
}