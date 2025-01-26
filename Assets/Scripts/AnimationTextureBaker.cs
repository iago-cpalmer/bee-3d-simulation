using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    class AnimationTextureBaker : MonoBehaviour
    {
        [SerializeField] private bool bake;
        [SerializeField] private AnimationClip[] clips;


        private void Start()
        {
            
        }

        private void OnValidate()
        {
            if(bake)
            {
                // Bake animations to textures
                foreach(AnimationClip clip in clips)
                {
                    BakeAnimation(clip);
                }
                bake = false;
            }
        }

        private void BakeAnimation(AnimationClip clip)
        {
            int totalFrames = (int)(clip.length / clip.frameRate) + 1;
            int width = (int)math.sqrt(totalFrames);
            int height = width;
            Texture2D texture = new Texture2D(width, height);
            texture.wrapModeU = TextureWrapMode.Clamp;
            texture.wrapModeV = TextureWrapMode.Repeat;
            texture.filterMode = FilterMode.Bilinear;
            //Motion motion = 
        }

        private void SaveTexture(Texture2D texture, string textureName)
        {
            File.WriteAllBytes("Assets/BakedTextures/"+textureName+".png", texture.EncodeToPNG());
            //AssetDatabase.CreateAsset(texture, "Assets/BakedTextures/" + textureName + "_asset.png");
        }
    }
}
