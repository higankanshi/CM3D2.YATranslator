﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BepInEx.Logging;
using Kanro.MaidTranslate.Hook;
using Kanro.MaidTranslate.Util;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = BepInEx.Logger;

namespace Kanro.MaidTranslate.Translation
{
    public class TextureTranslationPool
    {
        public Dictionary<string, string> Resource { get; } = new Dictionary<string, string>();

        public void AddResource(string path)
        {
            var name = Path.GetFileNameWithoutExtension(path);
            if (Resource.ContainsKey(name))
            {
                Logger.Log(LogLevel.Debug, $"[MaidTranslate] Same name resource of {name} found, use the latest version.");
            }

            Resource[name] = path;
        }

        public bool Translate(Scene? scene, TextureTranslationEventArgs e, out TextureResource resource)
        {
            if (Resource.ContainsKey(e.Name))
            {
                var translationFile = Resource[e.Name];

                switch (Path.GetExtension(translationFile))
                {
                    case ".png":
                        resource = new TextureResource(1, 1, TextureFormat.ARGB32, null, File.ReadAllBytes(translationFile));
                        return true;
                    case ".tex":
                        resource = TexUntil.LoadTexture(e.Name, File.ReadAllBytes(translationFile));
                        if (resource == null)
                        {
                            return false;
                        }
                        return true;
                    default:
                        resource = null;
                        return false;
                }
            }
            resource = null;
            return false;
        }
    }
}