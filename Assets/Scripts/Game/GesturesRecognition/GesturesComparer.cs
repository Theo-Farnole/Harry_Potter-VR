﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.Inspector;
using Utils.Pattern;

namespace GesturesRecognition
{
    public class GesturesComparer : Singleton<GesturesComparer>
    {
        [SerializeField, EnumNamedArray(typeof(SpellType))] private List<MDOrientationArray> _spellGestures;

        [System.Serializable]
        public class MDOrientationArray
        {
            public Gesture[] gesture;

            public MDOrientationArray(Gesture[] gesture)
            {
                this.gesture = gesture;
            }
        }

        public SpellType? CompareGesture(Orientation[] gesture)
        {
            for (int spellIndex = 0; spellIndex < _spellGestures.Count; spellIndex++)
            {
                SpellType spellType = (SpellType)(spellIndex);

                for (int i = 0; i < _spellGestures[spellIndex].gesture.Length; i++)
                {
                    if (_spellGestures[i] == null)
                    {
                        Debug.LogError("Gesture isn't linked in GesturesComparer in SC_gameLogic!");
                        continue;
                    }

                    if (gesture.Length == 0)
                        return null;

                    if (IsGestureValid(gesture, _spellGestures[spellIndex].gesture[i].MainGesture))
                    {                        
                        return spellType;
                    }

                    foreach (var smoothedGesture in _spellGestures[spellIndex].gesture[i].SmoothedGestures)
                    {
                        if (IsGestureValid(gesture, smoothedGesture.orientations))
                        {                            
                            return spellType;
                        }
                    }
                }
            }

            return null;
        }

        bool IsGestureValid(Orientation[] userGesture, Orientation[] definedGesture)
        {
            for (int deltaPosition = 0; deltaPosition < userGesture.Length; deltaPosition++)
            {
                // browse defined gesture
                for (int j = 0; j < definedGesture.Length; j++)
                {
                    int indexUserGesture = deltaPosition + j;

                    // avoid out of bounds index
                    if (indexUserGesture >= userGesture.Length)
                    {
                        break;
                    }

                    // do defined gesture match w/ userGesture ?
                    if (userGesture[deltaPosition + j] == definedGesture[j])
                    {
                        // do we reach end of gesture ?
                        if (j == definedGesture.Length - 1)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return false;
        }

        void OnValidate()
        {
            //int enumLength = Enum.GetValues(typeof(SpellType)).Length;

            //if (_spellGestures.Count != enumLength)
            //{
            //    Array.Resize(ref _spellGestures, enumLength);
            //}
        }
    }
}
