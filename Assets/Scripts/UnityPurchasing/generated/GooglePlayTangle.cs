// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("6Gtlalroa2Bo6Gtraufj0zNY+zxa6GtIWmdsY0DsIuydZ2tra29qadakUu6vRpKs/auuvVXVY2LKYr93mW9GGmPqWafJBr2dtymD9EMOzFGyb2nkWczzpMC528fitCS60z093lEvvQXNzTFP7emSWzI+E9orLptKNeMbrt5zGU+3TFGL88F+kT9RF4gFB6OT/eoWmblLfVrOikwXMMbOrhDQsBipRjTi0ADlS/DRd1BoYPOTwozbltTkrrFtR/6vtxvCA5jEnopmPhEJdfWMLeDv29RiTRZtWe3grv9TTG/UgD1K7+Ohlmnm++mVTUAK4HG5k6xB6II7os5VweJq67vVm6jPS46mnN/kvivfg8J+H26rSfLLs/4jJEj0noJcf2hpa2pr");
        private static int[] order = new int[] { 1,1,8,7,6,10,10,11,8,11,11,13,12,13,14 };
        private static int key = 106;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
