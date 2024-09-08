using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Verse;

namespace JJK
{
	/*
	 * This file contains code derived from the TMagic mod by TorannD.
	 * Original source: https://github.com/TorannD/TMagic
	 * 
	 * TMagic is licensed under the BSD 3-Clause License:
	 * 
	 * Copyright (c) 2019, TorannD
	 * All rights reserved.
	 * 
	 * Redistribution and use in source and binary forms, with or without
	 * modification, are permitted provided that the following conditions are met:
	 * 
	 * 1. Redistributions of source code must retain the above copyright notice, this
	 *    list of conditions and the following disclaimer.
	 * 
	 * 2. Redistributions in binary form must reproduce the above copyright notice,
	 *    this list of conditions and the following disclaimer in the documentation
	 *    and/or other materials provided with the distribution.
	 * 
	 * 3. Neither the name of the copyright holder nor the names of its
	 *    contributors may be used to endorse or promote products derived from
	 *    this software without specific prior written permission.
	 * 
	 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
	 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
	 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
	 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
	 * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
	 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
	 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
	 * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
	 * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
	 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
	 */
	[StaticConstructorOnStartup]
		public class Graphic_Animated : Graphic
		{
			public override void Init(GraphicRequest req)
			{
				this.data = req.graphicData;
				if (req.path.NullOrEmpty())
				{
					throw new ArgumentNullException("folderPath");
				}
				if (req.shader == null)
				{
					throw new ArgumentNullException("shader");
				}
				this.path = req.path;
				this.color = req.color;
				GraphicDataAnimation graphicDataAnimation = this.data as GraphicDataAnimation;
				if (graphicDataAnimation != null)
				{
					this.color.a = graphicDataAnimation.transparency;
					this.colorTwo.a = graphicDataAnimation.transparency;
				}
				this.colorTwo = req.colorTwo;
				this.drawSize = req.drawSize;
				List<Texture2D> list = (from x in ContentFinder<Texture2D>.GetAllInFolder(req.path)
										where !x.name.EndsWith(Graphic_Single.MaskSuffix)
										orderby x.name
										select x).ToList<Texture2D>();
				if (list.NullOrEmpty<Texture2D>())
				{
					Log.Error("Collection cannot init: No textures found at path " + req.path);
					this.subGraphics = new Graphic[]
					{
					BaseContent.BadGraphic
					};
					return;
				}
				this.subGraphics = new Graphic[list.Count];
				for (int i = 0; i < list.Count; i++)
				{
					string path = req.path + "/" + list[i].name;
					this.subGraphics[i] = GraphicDatabase.Get(typeof(Graphic_Single), path, req.shader, this.drawSize, this.color, this.colorTwo, null, req.shaderParameters, null);
				}
			}

	
			public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
			{
				if (this.animated == null || this.animated.AnimationDef == null)
				{
					base.DrawWorker(loc, rot, thingDef, thing, extraRotation);
					return;
				}
				Vector2? sizeOverride = this.animated.SizeOverride;
				Vector3 s = (sizeOverride != null) ? new Vector3(sizeOverride.Value.x, 1f, sizeOverride.Value.y) : new Vector3(1f, 1f, 1f);
				Vector3 exactScale = this.animated.ExactScale;
				s.x *= exactScale.x;
				s.z *= exactScale.z;
				Matrix4x4 matrix = default(Matrix4x4);
				if (!thingDef.rotatable)
				{
					rot = Rot4.North;
				}
				Quaternion q = (this.animated.Angle != 0f) ? Quaternion.AngleAxis(this.animated.Angle, Vector3.up) : rot.AsQuat;
				//if (this is Graphic_Animation_Multi)
				//{
				//	q = Quaternion.identity;
				//}
				matrix.SetTRS(loc, q, s);
				Graphic graphic = this.subGraphics[this.animated.CurInd];
				Material material = graphic.MatAt(rot, thing);
				Mote_Animation mote_Animation = this.animated as Mote_Animation;
				if (mote_Animation != null)
				{
					float alpha = mote_Animation.Alpha;
					Color color = this.color * mote_Animation.instanceColor;
					color.a *= alpha;
					if (!color.IndistinguishableFrom(material.color))
					{
						this.propertyBlock.SetColor(ShaderPropertyIDs.Color, color);
					}
				}
				Graphics.DrawMesh(graphic.MeshAt(rot), matrix, material, 0, null, 0, this.propertyBlock);
				if (this.animated.PrevTick != Find.TickManager.TicksGame && ((this.animated.AnimationDef.lockToRealTime && Time.frameCount % this.animated.AnimationDef.fpsRate == 0) || (!this.animated.AnimationDef.lockToRealTime && Find.TickManager.TicksGame % this.animated.AnimationDef.fpsRate == 0)))
				{
					if (this.animated.CurInd < this.subGraphics.Length - 1)
					{
						IAnimated animated = this.animated;
						int num = animated.CurInd;
						animated.CurInd = num + 1;
					}
					if (this.animated.CurInd >= this.subGraphics.Length - 1)
					{
						IAnimated animated2 = this.animated;
						int num = animated2.CurLoopInd;
						animated2.CurLoopInd = num + 1;
						this.animated.OnCycle_Completion();
						if (this.animated.AnimationDef.maxLoopCount > this.animated.CurLoopInd || this.animated.AnimationDef.maxLoopCount <= 0)
						{
							this.animated.CurInd = 0;
						}
					}
				}
				this.animated.PrevTick = Find.TickManager.TicksGame;
			}

			public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
			{
				return GraphicDatabase.Get<Graphic_Animated>(this.path, newShader, this.drawSize, newColor, newColorTwo, this.data, null);
			}

			public Graphic[] subGraphics;
			public IAnimated animated;
			protected MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
		}

		public class GraphicDataAnimation : GraphicData
		{
			public Material[] Materials
			{
				get
				{
					if (this.cachedMaterials == null)
					{
						this.InitMainTextures();
					}
					return this.cachedMaterials;
				}
			}


			public void InitMainTextures()
			{
				Material[] array;
				if (!GraphicDataAnimation.loadedMaterials.TryGetValue(this.texPath, out array))
				{
					List<string> list = (from x in this.LoadAllFiles(this.texPath)
										 orderby x
										 select x).ToList<string>();
					if (list.Count > 0)
					{
						this.cachedMaterials = new Material[list.Count];
						for (int i = 0; i < list.Count; i++)
						{
							Shader shader = (this.shaderType != null) ? this.shaderType.Shader : ShaderDatabase.DefaultShader;
							this.cachedMaterials[i] = MaterialPool.MatFrom(list[i], shader, this.color);
						}
					}
					GraphicDataAnimation.loadedMaterials[this.texPath] = this.cachedMaterials;
					return;
				}
				this.cachedMaterials = array;
			}


			public List<string> LoadAllFiles(string folderPath)
			{
				List<string> list = new List<string>();
				foreach (ModContentPack mod in LoadedModManager.RunningModsListForReading)
				{
					foreach (KeyValuePair<string, FileInfo> keyValuePair in ModContentPack.GetAllFilesForMod(mod, "Textures/" + folderPath, null, null))
					{
						string text = keyValuePair.Value.FullName;
						if (text.EndsWith(".png"))
						{
							text = text.Replace("\\", "/");
							text = text.Substring(text.IndexOf("/Textures/") + 10);
							text = text.Replace(".png", "");
							list.Add(text);
						}
					}
				}
				return list;
			}

			public float transparency = 1f;

			public float animationSpeedRate = 1f;

			private Material[] cachedMaterials;
			private static Dictionary<string, Material[]> loadedMaterials = new Dictionary<string, Material[]>();
		}
	}
