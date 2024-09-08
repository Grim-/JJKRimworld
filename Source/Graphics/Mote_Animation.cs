using RimWorld;
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
	public class Mote_Animation : MoteAttached, IAnimated
	{

			public float Angle
			{
				get
				{
					if (this.AnimationDef.angleOffset == 0f)
					{
						return this.angle;
					}
					return this.AnimationDef.angleOffset;
				}
			}

			public int CurInd
			{
				get
				{
					return this.curInd;
				}
				set
				{
					this.curInd = value;
				}
			}

			public int CurLoopInd
			{
				get
				{
					return this.curLoopInd;
				}
				set
				{
					this.curLoopInd = value;
				}
			}

			public int PrevTick
			{
				get
				{
					return this.prevTick;
				}
				set
				{
					this.prevTick = value;
				}
			}

			public virtual Vector2? SizeOverride
			{
				get
				{
					return this.sizeOverride;
				}
			}

			protected override bool EndOfLife
			{
				get
				{
					return false;
				}
			}

			public override void SpawnSetup(Map map, bool respawningAfterLoad)
			{
				base.SpawnSetup(map, respawningAfterLoad);
				if (!respawningAfterLoad)
				{
					this.activatedTick = Find.TickManager.TicksGame;
					string msg = "Creating animation: " + ((this != null) ? this.ToString() : null);
					Ability ability = this.sourceAbility;
				}
			}

			public virtual void OnCycle_Completion()
			{
				if (!this.destroy && this.CurLoopInd >= this.AnimationDef.maxLoopCount && (this.endLoop || this.AnimationDef.maxLoopCount > 0))
				{
					this.destroy = true;
					if (this.AnimationDef.additionalLifetimeTicks > 0)
					{
						this.expireInTick = this.AnimationDef.additionalLifetimeTicks;
						this.activatedTick = Find.TickManager.TicksGame;
					}
				}
			}


			public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
			{
				this.DoAction();
				string msg = "Destroying animation: " + ((this != null) ? this.ToString() : null);
				Ability ability = this.sourceAbility;

				base.Destroy(mode);
				//if (this.sourceAbility != null)
				//{
				//	this.sourceAbility.animations.Remove(this);
				//	Pawn pawn = this.sourceAbility.pawn;
				//	if (pawn == null)
				//	{
				//		return;
				//	}
				//	//pawn.();
				//}
			}

			public virtual void DoAction()
			{
				if (this.AnimationDef.spawnOverlayOnEnd != null)
				{
					MoteAttached moteAttached = GenSpawn.Spawn(this.AnimationDef.spawnOverlayOnEnd, base.Position, base.Map, WipeMode.Vanish) as MoteAttached;
					if (moteAttached != null)
					{
						moteAttached.Attach(this.link1.Target);
					}
				}
			}

			protected override void DrawAt(Vector3 drawLoc, bool flip = false)
			{
				this.DrawCustom(this.AnimationDef.altitudeLayer.AltitudeFor());
			}

			public AnimationDef AnimationDef
			{
				get
				{
					return this.def as AnimationDef;
				}
			}

			public new Graphic DefaultGraphic
			{
				get
				{
					if (this.graphicInt == null)
					{
						if (this.AnimationDef.graphicData == null)
						{
							return BaseContent.BadGraphic;
						}
						this.graphicInt = this.AnimationDef.graphicData.GraphicColoredFor(this);
					}
					return this.graphicInt;
				}
			}

			public override Graphic Graphic
			{
				get
				{
					return this.DefaultGraphic;
				}
			}

			public Graphic_Animated Graphic_Animation
			{
				get
				{
					return this.Graphic as Graphic_Animated;
				}
			}

			public virtual void DrawCustom(float altitude)
			{
				this.exactPosition.y = altitude;
				if (this.AnimationDef.attachedRotationDraw != null)
				{
					Rot4 rotation = this.link1.Target.Thing.Rotation;
					if (!this.AnimationDef.attachedRotationDraw.Contains(rotation))
					{
						return;
					}
				}
				Vector3 loc = this.DrawPos + this.AnimationDef.graphicData.drawOffset;
				Graphic_Animated graphic_Animation = this.Graphic as Graphic_Animated;
				if (graphic_Animation != null)
				{
					graphic_Animation.animated = this;
				}
				if (this.link1.Target.IsValid)
				{
					this.Graphic.DrawWorker(loc, this.link1.Target.Thing.Rotation, this.def, this, 0f);
					return;
				}
				this.Graphic.DrawWorker(loc, base.Rotation, this.def, this, 0f);
			}

			public override void Tick()
			{
				base.Tick();
				if (!base.Destroyed)
				{
					if (this.destroy && this.expireInTick <= 0)
					{
						this.Destroy(DestroyMode.Vanish);
						return;
					}
					if (this.expireInTick > 0 && Find.TickManager.TicksGame >= this.activatedTick + this.expireInTick)
					{
						this.Destroy(DestroyMode.Vanish);
						return;
					}
					if (this.expireInTick <= 0 && this.AnimationDef.maxLoopCount <= 0)
					{
						if (this.sourceHediff != null && this.sourceHediff.pawn.health.hediffSet.hediffs.Contains(this.sourceHediff))
						{
							return;
						}
						this.Destroy(DestroyMode.Vanish);
					}
				}
			}

			public override void ExposeData()
			{
				base.ExposeData();
				Scribe_Values.Look<int>(ref this.curInd, "curInd", 0, false);
				Scribe_Values.Look<int>(ref this.curLoopInd, "curLoopInd", 0, false);
				Scribe_Values.Look<Vector2?>(ref this.sizeOverride, "sizeOverride", null, false);
				Scribe_Values.Look<Vector3>(ref this.linearScale, "linearScale", default(Vector3), false);
				Scribe_Values.Look<Vector3>(ref this.curvedScale, "curvedScale", default(Vector3), false);
				Scribe_Values.Look<int>(ref this.activatedTick, "activatedTick", 0, false);
				Scribe_Values.Look<int>(ref this.expireInTick, "expireInTick", 0, false);
				Scribe_References.Look<Ability>(ref this.sourceAbility, "ability", false);
				Scribe_References.Look<Hediff>(ref this.sourceHediff, "sourceHediff", false);
				Scribe_Values.Look<float>(ref this.angle, "angle", 0f, false);
				Scribe_Values.Look<int>(ref this.prevTick, "prevTick", 0, false);
				Scribe_Values.Look<Vector3>(ref this.exactPosition, "exactPosition", default(Vector3), false);
				Scribe_Values.Look<float>(ref this.exactRotation, "exactRotation", 0f, false);
				Scribe_Values.Look<float>(ref this.rotationRate, "rotationRate", 0f, false);
				Scribe_Values.Look<float>(ref this.yOffset, "yOffset", 0f, false);
				Scribe_Values.Look<Color>(ref this.instanceColor, "instanceColor", Color.white, false);
				//Scribe_Values.Look<int>(ref this.lastMaintainTick, "lastMaintainTick", 0, false);
				//Scribe_Values.Look<int>(ref this.currentAnimationTick, "currentAnimationTick", 0, false);
				Scribe_Values.Look<float>(ref this.solidTimeOverride, "solidTimeOverride", -1f, false);
				Scribe_Values.Look<int>(ref this.pausedTicks, "pausedTicks", 0, false);
				Scribe_Values.Look<bool>(ref this.paused, "paused", false, false);
				Scribe_Values.Look<int>(ref this.spawnTick, "spawnTick", 0, false);
				Scribe_Values.Look<bool>(ref this.animationPaused, "animationPaused", false, false);
				Scribe_Values.Look<int>(ref this.detachAfterTicks, "detachAfterTicks", -1, false);
				Scribe_Values.Look<float>(ref this.spawnRealTime, "spawnRealTime", 0f, false);
				//if (this.link1.Target.HasThing)
				//{
				//	this.target = this.link1.Target;
				//	this.offset = this.link1.offsetInt;
				//}
				Scribe_Values.Look<Vector3>(ref this.offset, "offset", default(Vector3), false);
				Scribe_TargetInfo.Look(ref this.target, "target");
				if (this.target.IsValid)
				{
					this.link1.UpdateTarget(this.target, this.offset);
				}
			}

			public int curInd;
			protected int curLoopInd;
			protected int prevTick;
			public float angle;
			public Vector2? sizeOverride;
			protected bool destroy;
			public bool endLoop;
			public int expireInTick;
			public int activatedTick;
			public Ability sourceAbility;
			public Hediff sourceHediff;
			private Graphic graphicInt;
			private TargetInfo target;
			private Vector3 offset;
		}
	}
