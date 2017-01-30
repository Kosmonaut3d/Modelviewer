using Microsoft.Xna.Framework;

namespace HelperSuite.HelperSuite.Static
{
    public static class GameSettings
    {
        public static int ShowDisplayInfo = 3;
        
        public static int g_ScreenWidth = 1280;
        public static int g_ScreenHeight = 800;
        public static bool ui_DrawUI = true;

        public static bool ui_debug = true;
        

        public static Color bgColor = Color.White;
        public static float g_FarPlane = 500;
        public static bool RotateOrbit = true;
        public static bool m_orientationy = true;
        public static float m_size = 1;
        public static float m_roughness = 0.3f;
        public static float m_metallic = 0.3f;

        public static bool r_UseLinear = true;
        public static bool m_startClip = true;
        public static bool m_updateAnimation = true;
    }
}
