using game.management;

namespace ballfall.management {
    public class BallFallGame : Game {
        FallScene _fallScene;

        public BallFallGame (IUtil util, IContentManager contentManager) : base (util, contentManager) {
            _fallScene = new FallScene ();
        }

        public override void Init (int width, int height) {
            base.Init (width, height);
            CurrentScene = _fallScene;
        }
    }
}