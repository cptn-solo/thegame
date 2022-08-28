using Assets.Scripts.Services.App;
using Fusion;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class HUDLeaderBoardView : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup layoutGroup;
        [SerializeField] private GameObject itemPrefab;
        
        private readonly Dictionary<NetworkId, LeaderBoardItemView> leaders = new();



        internal void AddPlayer(NetworkId id)
        {
            var playerView = Instantiate(itemPrefab).GetComponent<LeaderBoardItemView>();
            var listRectTransform = layoutGroup.GetComponent<RectTransform>();
            playerView.Attach(listRectTransform, id);
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
                leaderView.SetInfo(playerInfo, localPlayer);
        }
    }
}