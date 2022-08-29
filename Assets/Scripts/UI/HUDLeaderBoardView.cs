using Assets.Scripts.Services.App;
using Fusion;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class HUDLeaderBoardView : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup layoutGroup;
        [SerializeField] private GameObject itemPrefab;
        
        private RectTransform rectTransform;

        private readonly Dictionary<NetworkId, LeaderBoardItemView> leaders = new();

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        internal void AddPlayer(NetworkId id)
        {
            var playerView = Instantiate(itemPrefab).GetComponent<LeaderBoardItemView>();
            var listRectTransform = layoutGroup.GetComponent<RectTransform>();
            playerView.Attach(listRectTransform, id, rectTransform.localScale.x);
            playerView.SetInfo(new PlayerInfo(), false);
            leaders.Add(id, playerView);
        }
        internal void RemovePlayer(NetworkId id)
        {
            if (leaders.TryGetValue(id, out var leaderView) && !leaderView.gameObject.IsDestroyed())
            {
                leaderView.Detach();
                Destroy(leaderView.gameObject);
            }
        }

        internal void UpdatePlayer(NetworkId id, PlayerInfo playerInfo, bool localPlayer)
        {
            if (leaders.TryGetValue(id, out var leaderView) && !leaderView.gameObject.IsDestroyed())
            {
                leaderView.SetInfo(playerInfo, localPlayer);

                var sorted = leaders.Values.OrderByDescending(x => x.Info.Score).ToArray();

                for (int idx = 0; idx < sorted.Count(); idx++)
                    sorted[idx].SetListPosition(idx);
            }
        }
    }
}