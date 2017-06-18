﻿using ICities;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SingleTrackAI
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private bool hookEnabled = false;  
        private Dictionary<MethodInfo, RedirectCallsState> redirects = new Dictionary<MethodInfo, RedirectCallsState>();

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
        }


        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            EnableHook();
            SingleTrainTrackAI.Initialize();
        }
		

        public override void OnReleased()
        {
            base.OnReleased();
            DisableHook();
        }

        public void EnableHook()
        {
            if (hookEnabled)
            {
                return;
            }
            var allFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
            var method = typeof(TrainAI).GetMethod("CheckNextLane", allFlags);
            redirects.Add(method, RedirectionHelper.RedirectCalls(method, typeof(SingleTrainTrackAI).GetMethod("CheckNextLane", allFlags)));

            /*method = typeof(NetSegment).GetMethods(allFlags).Single(c => c.Name == "CalculateCorner" && c.GetParameters().Length == 20);
            CODebug.Log(LogChannel.Modding, "redirects method : " + method);
            redirects.Add(method, RedirectionHelper.RedirectCalls(method, typeof(SingleTrainTrackAI).GetMethod("CalculateCorner", allFlags)));
            */

            hookEnabled = true;
        }

        public void DisableHook()
        {
            if (!hookEnabled)
            {
                return;
            }
            foreach (var kvp in redirects)
            {

                RedirectionHelper.RevertRedirect(kvp.Key, kvp.Value);
            }
            redirects.Clear();
            hookEnabled = false;
        }
    }
}