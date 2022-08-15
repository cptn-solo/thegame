using Fusion;

namespace Assets.Scripts.Views
{
    public class Despawnable : NetworkBehaviour
    {
        [Networked] private TickTimer life { get; set; }
        public void InitForLifeTime(float lifeTime)
        {
            this.life = TickTimer.CreateFromSeconds(Runner, lifeTime);
        }

        public override void FixedUpdateNetwork()
        {
            if (Runner.Stage == SimulationStages.Forward && life.Expired(Runner))
                Runner.Despawn(Object);
        }
    }
}
