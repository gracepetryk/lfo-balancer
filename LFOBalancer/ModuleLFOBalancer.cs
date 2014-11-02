using System;
using UnityEngine;

namespace LFOBalancer {
	// TODO: Add more comments
	public class ModuleLFOBalancer : PartModule {
		public bool balance = false;
		public const string ENABLED_STRING = "Enable Oxidizer Balance";
		public const string DISABLED_STRING = "Disable Oxidizer Balance";

		
		public override void OnUpdate () {
			if (balance) {
				balanceFuel();
			}
		}

		// we need a seperate method because KSPEvent only calls once but we need it to run continuously
		[KSPEvent(active=true, guiActive=true, guiActiveUnfocused=false, guiName=ENABLED_STRING)]
		public void enableBalance() {
			balance = true;
			Events["enableBalance"].active = false;
			Events["disableBalance"].active = true;
		}
		
		[KSPEvent(active = false, guiActive = true, guiName = DISABLED_STRING)]
		public void disableBalance(){
			balance = false;
			Events["enableBalance"].active = true;
			Events["disableBalance"].active = false;
		}
		
		public void balanceFuel() {
			PartResource oxy;
			PartResource lf;
			double idealOxy;
			
			foreach(Part localPart in this.vessel.Parts) {
				if (localPart.Resources.Contains("Oxidizer") && localPart.Resources.Contains ("LiquidFuel")) {	
					lf = localPart.Resources["LiquidFuel"];
					oxy = localPart.Resources["Oxidizer"];
					
					idealOxy = (oxy.maxAmount * lf.amount) / lf.maxAmount;
					
					// make sure we dont create new fuel
					if (!idealOxy > oxy.amount) {
						oxy.amount = idealOxy;
					}
				}
			}
		}
	}
}