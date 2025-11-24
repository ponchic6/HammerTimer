using System;
using Code.Infrastructure.View;
using DG.Tweening;
using Entitas;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Gameplay.Produce.ReleaseZone.View
{
    public class ReleaseZoneView : MonoBehaviour
    {
        [SerializeField] private EntityBehaviour entityBehaviour;
        [SerializeField] private TMP_Text reward;
        [SerializeField] private float moveUpDistance;
        [SerializeField] private float animationDuration;
        private IGroup<GameEntity> _releasedAsOrderGroup;
        private Vector3 _rewardStartPosition;
        private GameContext _game;

        private void Start()
        {
            _game = Contexts.sharedInstance.game;
            _rewardStartPosition = reward.transform.localPosition;
            reward.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_game.isReleasedAsOrder)
            {
                PlayRewardAnimation();
            }
        }
        
        private void PlayRewardAnimation()
        {
            int rewardAmount = Random.Range(1, 101);
            reward.text = $"${rewardAmount}";

            reward.transform.localPosition = _rewardStartPosition;
            reward.alpha = 0f;
            reward.gameObject.SetActive(true);

            Sequence rewardSequence = DOTween.Sequence();
            rewardSequence.Append(reward.DOFade(1f, animationDuration * 0.2f));
            rewardSequence.Join(reward.transform.DOLocalMoveY(_rewardStartPosition.y + moveUpDistance, animationDuration).SetEase(Ease.OutQuad));
            rewardSequence.Append(reward.DOFade(0f, animationDuration * 0.3f));
            rewardSequence.OnComplete(() =>
            {
                reward.gameObject.SetActive(false);
                reward.transform.localPosition = _rewardStartPosition;
            });
        }
    }
}