using game.management;

namespace ballfall.management {
    public class BallFallGame : Game {
        #region Static data
        FallScene _fallScene;
        #endregion

        #region Dynamic data
        #endregion

        #region Construction
        public BallFallGame (IUtil util, IContentManager contentManager) : base (util, contentManager) {
            _fallScene = new FallScene (this);
        }
        #endregion

        #region Interface
        public override void Init (int width, int height) {
            base.Init (width, height);
            CurrentScene = _fallScene;
        }
        #endregion
    }
}