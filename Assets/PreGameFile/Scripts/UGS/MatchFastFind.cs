using UnityEngine;
namespace Unity.Megacity.UGS
{
    public class MatchFastFind : MonoBehaviour
    {
        public void FindMatch()
        {
            Matchmake();
        }
        
        private async void Matchmake()
        {
            await MatchMakingConnector.Instance.Matchmake();
        }
    }
}