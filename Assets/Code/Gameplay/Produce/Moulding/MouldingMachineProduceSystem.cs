using System;
using System.Collections.Generic;
using Code.Infrastructure.StaticData;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Produce.Moulding
{
    public class MouldingCalculateQualitySystem : IExecuteSystem
    {
        private readonly GameContext _game;
        private readonly IGroup<GameEntity> _entities;
        private readonly CommonStaticData _commonStaticData;
        private List<GameEntity> _buffer = new(16);

        public MouldingCalculateQualitySystem(CommonStaticData commonStaticData)
        {
            _commonStaticData = commonStaticData;
            _game = Contexts.sharedInstance.game;

            _entities = _game.GetGroup(GameMatcher.AllOf(GameMatcher.MouldingMachine, GameMatcher.MouldingQuality));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities.GetEntities(_buffer))
            {
                float delta = (float)(DateTime.Now.TimeOfDay - entity.mouldingQuality.StartTime).TotalSeconds;
                
                float curveMinTime = _commonStaticData.qualityTimeCurve.keys[0].time;
                float curveMaxTime = _commonStaticData.qualityTimeCurve.keys[^1].time;
                float clampedDelta = Mathf.Clamp(delta, curveMinTime, curveMaxTime);

                float evaluatedQuality = _commonStaticData.qualityTimeCurve.Evaluate(clampedDelta);
                entity.mouldingQuality.Quality = evaluatedQuality;
            }
        }
    }
}