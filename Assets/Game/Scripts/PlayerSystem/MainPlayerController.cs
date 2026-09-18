using System.Collections;
using Game.Scripts.Core.PlayerSystem;
using Game.Scripts.PlayerSystem.BuilderSystem;
using Game.Scripts.PlayerSystem.PawnSystem;
using UnityEngine;

namespace Game.Scripts.PlayerSystem
{
    public class MainPlayerController : PlayerController
    {
        [SerializeField] private PlayerPawnController pawn;
        [SerializeField] private PlayerBuilderController builder;

        public override IEnumerator EnablePlayer(CameraController camera)
        {
            yield return base.EnablePlayer(camera);

            yield return pawn.Activate();
            yield return builder.Activate();
        }

        public override void DisablePlayer()
        {
            pawn.Deactivate();
            builder.Deactivate();
            
            base.DisablePlayer();
        }
    }
}
