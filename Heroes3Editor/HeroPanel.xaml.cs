﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Heroes3Editor.Models;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.ComponentModel;

namespace Heroes3Editor
{
    /// <summary>
    /// Interaction logic for HeroPanel.xaml
    /// </summary>
    public partial class HeroPanel : UserControl
    {
        private Hero _hero;

        public Hero Hero
        {
            set
            {
                _hero = value;
                if (_hero.IsHOTAGame)
                {
                    SetHOTASettings();
                }
                else { 
                    SetClassicSettings();
                }
                for (int i = 0; i < 4; ++i)
                {
                    var txtBox = FindName("Attribute" + i) as TextBox;
                    txtBox.Text = _hero.Attributes[i].ToString();
                }

                for (int i = 0; i < 8; ++i)
                {
                    var cboBox = FindName("Skill" + i) as ComboBox;
                    var txtBox = FindName("SkillLevel" + i) as TextBox;
                    if (i < _hero.NumOfSkills)
                    {
                        cboBox.SelectedItem = _hero.Skills[i];
                        txtBox.Text = _hero.SkillLevels[i].ToString();
                    }
                    else if (i > _hero.NumOfSkills)
                    {
                        cboBox.IsEnabled = false;
                        txtBox.IsEnabled = false;
                    }
                    else
                    {
                        txtBox.IsEnabled = false;
                    }
                }

                foreach (var spell in _hero.Spells)
                {
                    var chkBox = FindName(spell) as CheckBox;
                    chkBox.IsChecked = true;
                }

                for (int i = 0; i < 7; ++i)
                {
                    var cboBox = FindName("Creature" + i) as ComboBox;
                    var txtBox = FindName("CreatureAmount" + i) as TextBox;
                    if (_hero.Creatures[i] != null)
                    {
                        cboBox.SelectedItem = _hero.Creatures[i];
                        txtBox.Text = _hero.CreatureAmounts[i].ToString();
                    }
                    else
                    {
                        txtBox.IsEnabled = false;
                    }
                }

                foreach (var warMachine in _hero.WarMachines)
                {
                    var toggleComponent = FindName(warMachine) as ToggleButton;
                    toggleComponent.IsChecked = true;
                }

                var gears = new List<string>(_hero.EquippedArtifacts.Keys);
                foreach (var gear in gears)
                {
                    // Attach an EA_ prefix to gear because there's already
                    // a CheckBox for the spell Shield
                    var cboBox = FindName("EA_" + gear) as ComboBox;
                    cboBox.SelectedItem = _hero.EquippedArtifacts[gear];
                }
            }
        }

        public HeroPanel()
        {
            InitializeComponent();
        }

        private void SetHOTASettings()
        {
            SetComponentVisibility("Ballista", Visibility.Hidden);
            SetComponentVisibility("BallistaRadio", Visibility.Visible);
            SetComponentVisibility("Canon", Visibility.Visible);
        }
        private void SetClassicSettings()
        {
            SetComponentVisibility("Ballista", Visibility.Visible);
            SetComponentVisibility("BallistaRadio", Visibility.Hidden);
            SetComponentVisibility("Canon", Visibility.Hidden);
        }
        private void SetComponentVisibility(string name, Visibility visibility )
        {
            var component = FindName(name) as ButtonBase;
            if (component != null)
            {
                component.Visibility = visibility;
            }
        }
        private void UpdateAttribute(object sender, RoutedEventArgs e)
        {
            var txtBox = e.Source as TextBox;

            byte value;
            bool isNumber = byte.TryParse(txtBox.Text, out value);
            if (!isNumber || value < 0 || value > 99) return;

            var i = int.Parse(txtBox.Name.Substring("Attribute".Length));
            _hero.UpdateAttribute(i, value);
        }

        private void UpdateSkill(object sender, RoutedEventArgs e)
        {
            var cboBox = e.Source as ComboBox;
            var slot = int.Parse(cboBox.Name.Substring("Skill".Length));
            var skill = cboBox.SelectedItem as string;

            var oldNumOfSkills = _hero.NumOfSkills;
            _hero.UpdateSkill(slot, skill);

            if (_hero.NumOfSkills > oldNumOfSkills)
            {
                var txtBox = FindName("SkillLevel" + slot) as TextBox;
                txtBox.Text = _hero.SkillLevels[slot].ToString();
                txtBox.IsEnabled = true;

                if (_hero.NumOfSkills < 8)
                {
                    var nextCboBox = FindName("Skill" + _hero.NumOfSkills) as ComboBox;
                    nextCboBox.IsEnabled = true;
                }
            }
        }

        private void UpdateSkillLevel(object sender, RoutedEventArgs e)
        {
            var txtBox = e.Source as TextBox;
            var slot = int.Parse(txtBox.Name.Substring("SkillLevel".Length));

            byte level;
            bool isNumber = byte.TryParse(txtBox.Text, out level);
            if (!isNumber || level < 0 || level > 3) return;

            _hero.UpdateSkillLevel(slot, level);
        }

        private void AddSpell(object sender, RoutedEventArgs e)
        {
            var chkBox = e.Source as CheckBox;
            _hero.AddSpell(chkBox.Name);
        }

        private void RemoveSpell(object sender, RoutedEventArgs e)
        {
            var chkBox = e.Source as CheckBox;
            _hero.RemoveSpell(chkBox.Name);
        }

        private void UpdateCreature(object sender, RoutedEventArgs e)
        {
            var cboBox = e.Source as ComboBox;
            var i = int.Parse(cboBox.Name.Substring("Creature".Length));
            var creature = cboBox.SelectedItem as string;

            _hero.UpdateCreature(i, creature);
            var txtBox = FindName("CreatureAmount" + i) as TextBox;
            if (!txtBox.IsEnabled)
            {
                txtBox.Text = _hero.CreatureAmounts[i].ToString();
                txtBox.IsEnabled = true;
            }
        }

        private void UpdateCreatureAmount(object sender, RoutedEventArgs e)
        {
            var txtBox = e.Source as TextBox;
            var i = int.Parse(txtBox.Name.Substring("CreatureAmount".Length));

            int amount;
            bool isNumber = int.TryParse(txtBox.Text, out amount);
            if (!isNumber || amount < 0 || amount > 9999) return;

            _hero.UpdateCreatureAmount(i, amount);
        }

        private void AddWarMachine(object sender, RoutedEventArgs e)
        {
            var component = e.Source as ButtonBase;
            if (component == null)
            {
                return;
            }
            _hero.AddWarMachine(component.Tag.ToString());
        }

        private void RemoveWarMachine(object sender, RoutedEventArgs e)
        {
            var component = e.Source as ButtonBase;
            _hero.RemoveWarMachine(component.Tag.ToString());
        }

        private void UpdateEquippedArtifact(object sender, RoutedEventArgs e)
        {
            var cboBox = e.Source as ComboBox;
            var gear = cboBox.Name.Substring("EA_".Length);
            var artifact = cboBox.SelectedItem as string;
            _hero.UpdateEquippedArtifact(gear, artifact);
        }

        private void UpdateArtifactInfo(object sender, RoutedEventArgs e)
        {
            var cboBox = e.Source as ComboBox;
            var artifact = cboBox.SelectedItem as string;

            if (null != _hero.UpdateArtifactInfo(artifact))
            {
                var txtBlock = FindName("Attack") as TextBlock;
                txtBlock.Text = _hero.UpdateArtifactInfo(artifact)[1];

                txtBlock = FindName("Defense") as TextBlock;
                txtBlock.Text = _hero.UpdateArtifactInfo(artifact)[2];

                txtBlock = FindName("Power") as TextBlock;
                txtBlock.Text = _hero.UpdateArtifactInfo(artifact)[3];

                txtBlock = FindName("Knowledge") as TextBlock;
                txtBlock.Text = _hero.UpdateArtifactInfo(artifact)[4];

                txtBlock = FindName("Morale") as TextBlock;
                txtBlock.Text = _hero.UpdateArtifactInfo(artifact)[5];

                txtBlock = FindName("Luck") as TextBlock;
                txtBlock.Text = _hero.UpdateArtifactInfo(artifact)[6];

                txtBlock = FindName("Effects") as TextBlock;
                txtBlock.Text = _hero.UpdateArtifactInfo(artifact)[7];
            }
        }

        private void ClearArtifactInfo(object sender, RoutedEventArgs e)
        {
            var txtBlock = FindName("Attack") as TextBlock;
            txtBlock.Text = "";
            txtBlock = FindName("Defense") as TextBlock;
            txtBlock.Text = "";
            txtBlock = FindName("Power") as TextBlock;
            txtBlock.Text = "";
            txtBlock = FindName("Knowledge") as TextBlock;
            txtBlock.Text = "";
            txtBlock = FindName("Morale") as TextBlock;
            txtBlock.Text = "";
            txtBlock = FindName("Luck") as TextBlock;
            txtBlock.Text = "";
            txtBlock = FindName("Effects") as TextBlock;
            txtBlock.Text = "";
        }
    }
}
