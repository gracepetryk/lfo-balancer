using System.Collections.Generic;
using Expansions.Missions.Actions;
using JetBrains.Annotations;
using KSP.UI.Screens;
using UnityEngine;

namespace LFOBalancer
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class Balancer : MonoBehaviour
    {
        private static readonly Texture Texture = null; // TODO: Add Texture
        [UsedImplicitly] private static ApplicationLauncherButton _button;
        
        
        private static bool _balance;
        
        private static List<ModuleLFOBalancer> _balancers = new List<ModuleLFOBalancer>();
        private static readonly List<PartResource> OxidizerList = new List<PartResource>();
        
        private double _maxOx;
        private double _currentLf;
        private double _currentOx;
        private double _desiredOx;

        private const double MAX_DUMP_PER_UPDATE = 0.1;
        
        // ratio of Oxidizer to LiquidFuel
        private const double FUEL_RATIO = 11.0 / 9.0;

        void Awake()
        {
            Debug.Log("LFO Balancer Initialized");
            
            InitializeButton();

            GameEvents.onVesselWasModified.Add(UpdateBalancerList);
            GameEvents.onVesselChange.Add(UpdateBalancerList);
        }

        private void FixedUpdate()
        {
            if (!_balance) return;
            
            
            UpdateResourceLists();

            _desiredOx = FUEL_RATIO * _currentLf;
            if (_currentOx <= _desiredOx) return; // don't create more fuel
                
                
            foreach (var oxy in OxidizerList)
            {
                var pctOfTotal = oxy.maxAmount / _maxOx; // of all the managed tanks, what is this ones proportion?
                var tankDesiredOxy = _desiredOx * pctOfTotal;
                                     
                if (!oxy._flowState) continue; // don't dump locked resources

                if (oxy.amount - tankDesiredOxy > MAX_DUMP_PER_UPDATE) // don't dump too fast
                {
                    oxy.amount -= MAX_DUMP_PER_UPDATE;
                }
                else
                {
                    oxy.amount = tankDesiredOxy;
                }
            }
        }
        
        private void OnDestroy()
        {
            //disconnect from game events
            GameEvents.onVesselWasModified.Remove(UpdateBalancerList);
            GameEvents.onVesselChange.Remove(UpdateBalancerList);
            ApplicationLauncher.Instance.RemoveModApplication(_button);
        }

        private void UpdateBalancerList(Vessel v)
        {
            _balancers.Clear();
            _balancers = v.FindPartModulesImplementing<ModuleLFOBalancer>();
            UpdateResourceLists();
        }
        
        private void UpdateResourceLists()
        {
            OxidizerList.Clear();
            _currentLf = 0;
            _maxOx = 0;

            foreach (var balancer in _balancers)
            {
                var lf = balancer.GetResource("LiquidFuel");
                var ox = balancer.GetResource("Oxidizer");

                if (lf == null || ox == null) continue;
                
                OxidizerList.Add(balancer.GetResource("Oxidizer"));
                
                _currentLf += lf.amount;
                
                _currentOx += ox.amount;
                _maxOx += ox.maxAmount;
            }
        }

        private static void InitializeButton()
        {
            _button = ApplicationLauncher.Instance.AddModApplication(
                OnTrue,
                OnFalse,
                null,
                null,
                null,
                null,
                ApplicationLauncher.AppScenes.FLIGHT,
                Texture
            );
        }
        
        private static void OnTrue() // called when button is pressed
        {
            Debug.Log("LFOBalancer: Enabled");
            _balance = true;
        }

        private static void OnFalse()
        {
            Debug.Log("LFOBalancer: Disabled");
            _balance = false;
        }
    }
}