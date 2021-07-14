using System;
using System.Linq;
using UnityEngine;

namespace CityBuilderCore
{
    /// <summary>
    /// dialog that displays information about the selected building
    /// </summary>
    public class SelectionDialog : DialogBase
    {
        public TMPro.TMP_Text TitleText;

        public GameObject DescriptionPanel;
        public TMPro.TMP_Text DescriptionText;

        public GameObject EvolutionPanel;
        public TMPro.TMP_Text EvolutionText;
        public ItemsPanel EvolutionItems;

        public GameObject EmploymentPanel;
        public TMPro.TMP_Text EmploymentText;

        public GameObject HousingPanel;
        public TMPro.TMP_Text HousingText;

        public GameObject ProductionPanel;
        public RectTransform ProductionBar;
        public ItemsPanel ProductionConsumerItems;
        public ItemsPanel ProductionProducerItems;
        private float _productionBarWidth;

        public GameObject StoragePanel;
        public StorageOrdersPanel StorageOrders;

        public GameObject DistributionPanel;
        public DistributionOrdersPanel DistributionOrders;

        public GameObject WorkerPanel;
        public WorkersPanel WorkingWorkers;
        public WorkersPanel QueuedWorkers;
        public WorkersPanel AssignedWorkers;

        public GameObject WalkerStoragePanel;
        public ItemsPanel WalkerStorageItems;

        public GameObject ItemEfficiencyPanel;
        public ItemsPanel EfficiencyItems;

        private object _currentTarget;

        protected override void Awake()
        {
            base.Awake();

            _productionBarWidth = ProductionBar.sizeDelta.x;
        }

        public void Activate(object target)
        {
            _currentTarget = target;

            base.Activate();
        }

        public override void Deactivate()
        {
            base.Deactivate();

            _currentTarget = null;
        }

        protected override void updateContent(bool initiate)
        {
            base.updateContent(initiate);

            var target = _currentTarget;

            if (target is UnityEngine.Object unityObject && !unityObject)
            {
                Deactivate();
                return;
            }

            if (target is BuildingReference reference)
                target = reference.Instance;

            if (target is Building building)
            {
                SetTitle(building.GetName());
                SetDescrition(building.GetDescription());

                SetEvolution(building.GetBuildingComponent<IEvolution>());
                SetEmployment(building.GetBuildingComponent<IEmployment>());
                SetHousing(building.GetBuildingComponent<IHousing>());
                SetProduction(building.GetBuildingComponent<IProductionComponent>());
                SetStorage(building.GetBuildingComponent<IStorageComponent>(), initiate);
                SetDistribution(building.GetBuildingComponent<IDistributionComponent>(), initiate);
                SetWorkerUser(building.GetBuildingComponent<IWorkerUser>());
                SetItemEfficiency(building.GetBuildingComponent<ItemEfficiencyComponent>());

                SetWalkerStorage(null);
            }
            else if (target is Walker walker)
            {
                SetTitle(walker.GetName());
                SetDescrition(walker.GetDescription());

                SetEvolution(null);
                SetEmployment(null);
                SetHousing(null);
                SetProduction(null);
                SetStorage(null, false);
                SetDistribution(null, false);
                SetWorkerUser(null);
                SetItemEfficiency(null);

                SetWalkerStorage(walker.ItemStorage);
            }
        }

        public void SetTitle(string title)
        {
            TitleText.text = title;
        }

        public void SetDescrition(string description)
        {
            DescriptionPanel.SetActive(true);
            DescriptionText.text = description;
        }

        public void SetEvolution(IEvolution evolution)
        {
            if (evolution == null)
            {
                EvolutionPanel.SetActive(false);
            }
            else
            {
                EvolutionPanel.SetActive(true);
                EvolutionText.text = evolution.GetDescription();
                EvolutionItems.SetItems(evolution.GetItems().ToList());
            }
        }

        public void SetEmployment(IEmployment employment)
        {
            if (employment == null)
            {
                EmploymentPanel.SetActive(false);
            }
            else
            {
                EmploymentPanel.SetActive(true);
                EmploymentText.text = employment.GetDescription();
            }
        }

        public void SetHousing(IHousing housing)
        {
            if (housing == null)
            {
                HousingPanel.SetActive(false);
            }
            else
            {
                HousingPanel.SetActive(true);
                HousingText.text = housing.GetDescription();
            }
        }

        public void SetProduction(IProductionComponent production)
        {
            if (production == null)
            {
                ProductionPanel.SetActive(false);
            }
            else
            {
                ProductionPanel.SetActive(true);

                ProductionBar.sizeDelta = new Vector2(_productionBarWidth * production.Progress, ProductionBar.sizeDelta.y);
                ProductionConsumerItems.SetItems(production.Consumers.Select(c => Tuple.Create(c.Items.Item, c.Storage)).ToList());
                ProductionProducerItems.SetItems(production.Producers.Select(c => Tuple.Create(c.Items.Item, c.Storage)).ToList());
            }
        }

        public void SetStorage(IStorageComponent storage, bool initiate)
        {
            if (storage == null)
            {
                StoragePanel.SetActive(false);
            }
            else
            {
                StoragePanel.SetActive(true);
                StorageOrders.SetOrders(storage, initiate);
            }
        }

        public void SetDistribution(IDistributionComponent distribution, bool initiate)
        {
            if (distribution == null)
            {
                DistributionPanel.SetActive(false);
            }
            else
            {
                DistributionPanel.SetActive(true);
                DistributionOrders.SetOrders(distribution, initiate);
            }
        }

        public void SetWorkerUser(IWorkerUser workerUser)
        {
            if (workerUser == null)
            {
                WorkerPanel.SetActive(false);
            }
            else
            {
                WorkerPanel.SetActive(true);
                WorkingWorkers.SetWorkers(workerUser.GetWorking());
                QueuedWorkers.SetWorkers(workerUser.GetQueued());
                AssignedWorkers.SetWorkers(workerUser.GetAssigned());
            }
        }

        public void SetWalkerStorage(ItemStorage storage)
        {
            if (storage == null)
            {
                WalkerStoragePanel.SetActive(false);
            }
            else
            {
                WalkerStoragePanel.SetActive(true);
                WalkerStorageItems.SetItems(storage);
            }
        }

        public void SetItemEfficiency(ItemEfficiencyComponent itemEfficiencyComponent)
        {
            if (itemEfficiencyComponent == null)
            {
                ItemEfficiencyPanel.SetActive(false);
            }
            else
            {
                ItemEfficiencyPanel.SetActive(true);
                EfficiencyItems.SetItems(itemEfficiencyComponent.GetItems().ToList());
            }
        }
    }
}