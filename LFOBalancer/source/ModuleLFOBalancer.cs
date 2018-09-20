using System.Runtime.CompilerServices;

namespace LFOBalancer {
	public class ModuleLFOBalancer : PartModule
	{
		private const string ENABLE_STRING = "Enable LFO Balancer";
		private const string DISABLE_STRING = "Disable LFO Balancer";

		[KSPField(isPersistant = true)] 
		public bool balance = true;


		// the next two methods handle toggling balance for individual tanks
		[KSPEvent(active = false, guiActive = true, guiActiveEditor = true, guiName = ENABLE_STRING, advancedTweakable = true)]
		public void EnableBalance()
		{
			balance = true;
			Events["DisableBalance"].active = true;
			Events["EnableBalance"].active = false;
		}

		[KSPEvent(active = true, guiActive = true, guiActiveEditor = true, guiName = DISABLE_STRING, advancedTweakable = true)]
		public void DisableBalance()
		{
			balance = false;
			Events["DisableBalance"].active = false;
			Events["EnableBalance"].active = true;
		}

		
		public PartResource GetResource(string resourceName)
		{
			// returns a part resource if the balancer is enabled
			PartResource resource;

			if (part.Resources.Contains(resourceName))
			{
				resource = part.Resources[resourceName];
			}
			else
			{
				return null;
			}

			return balance ? resource : null;
		}
	}
}