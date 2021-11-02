﻿using GAMMA.Toolbox;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace GAMMA.Models
{
    [Serializable]
    public class HitDiceSet : BaseModel
    {
        // Constructors
        public HitDiceSet()
        {
            HitDiceSides = Configuration.DiceSides;
        }

        // Databound Properties
        #region CurrentHitDice
        private int _CurrentHitDice;
        [XmlSaveMode("Single")]
        public int CurrentHitDice
        {
            get
            {
                return _CurrentHitDice;
            }
            set
            {
                _CurrentHitDice = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
        #region MaxHitDice
        private int _MaxHitDice;
        [XmlSaveMode("Single")]
        public int MaxHitDice
        {
            get
            {
                return _MaxHitDice;
            }
            set
            {
                _MaxHitDice = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
        #region HitDiceSides
        private List<string> _HitDiceSides;
        public List<string> HitDiceSides
        {
            get
            {
                return _HitDiceSides;
            }
            set
            {
                _HitDiceSides = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
        #region HitDiceQuality
        private int _HitDiceQuality;
        [XmlSaveMode("Single")]
        public int HitDiceQuality
        {
            get
            {
                return _HitDiceQuality;
            }
            set
            {
                _HitDiceQuality = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        // Commands
        #region RollHitDice
        public ICommand RollHitDice => new RelayCommand(DoRollHitDice);
        private void DoRollHitDice(object param)
        {
            if (param == null) { HelperMethods.WriteToLogFile("No parameter passed to HitDiceSet.RollHitDice", true); return; }

            CharacterModel RefChar = Configuration.MainModelRef.CharacterBuilderView.ActiveCharacter;
            if (CurrentHitDice <= 0) { HelperMethods.AddToPlayerLog(RefChar.Name + " has no more hit dice."); return; }
            if (RefChar.CurrentHealth == RefChar.MaxHealth) { HelperMethods.AddToPlayerLog(RefChar.Name + " is at max health already."); return; }

            string diceLimit = param.ToString();
            string message = "";
            int diceRoll;
            int total;
            List<int> rolls = new();
            int healTotal = 0;
            int modTotal = 0;
            switch (diceLimit)
            {
                case "Single":
                    HelperMethods.PlaySystemAudio(Configuration.SystemAudio_DiceRoll);
                    CurrentHitDice--;
                    diceRoll = Configuration.RNG.Next(1, HitDiceQuality + 1);
                    total = diceRoll + RefChar.ConstitutionModifier;
                    RefChar.CurrentHealth += total;
                    if (RefChar.CurrentHealth > RefChar.MaxHealth) { RefChar.CurrentHealth = RefChar.MaxHealth; }
                    message = RefChar.Name + " rolled a hit die.";
                    message += "\nResult: " + total + " hit point" + ((total > 1) ? "s" : "");
                    message += Configuration.MainModelRef.SettingsView.ShowDiceRolls ? "\nRoll: [" + diceRoll + "] + " + RefChar.ConstitutionModifier : "";
                    break;
                case "ToBloodied":
                    if (RefChar.Status == "Bloodied" || RefChar.Status == "Bruised" || RefChar.Status == "Fine")
                    {
                        HelperMethods.AddToPlayerLog("Character is already at Bloodied or better.");
                        return;
                    }
                    do
                    {
                        diceRoll = Configuration.RNG.Next(1, HitDiceQuality + 1);
                        total = diceRoll + RefChar.ConstitutionModifier;
                        RefChar.CurrentHealth += total;
                        rolls.Add(diceRoll);
                        modTotal += RefChar.ConstitutionModifier;
                        healTotal += total;
                        CurrentHitDice--;
                    }
                    while (Convert.ToDouble(RefChar.CurrentHealth) / Convert.ToDouble(RefChar.MaxHealth) < 0.25 && CurrentHitDice > 0);
                    message = RefChar.Name + " rolled hit dice to Bloodied.";
                    message += "\nResult: " + healTotal + " hit point" + ((healTotal > 1) ? "s" : "");
                    message += Configuration.MainModelRef.SettingsView.ShowDiceRolls ? "\nRoll: [" + HelperMethods.GetStringFromList(rolls, " + ") + "] + " + modTotal : "";
                    break;
                case "ToBruised":
                    if (RefChar.Status == "Bruised" || RefChar.Status == "Fine")
                    {
                        HelperMethods.AddToPlayerLog("Character is already at Bruised or better.");
                        return;
                    }
                    do
                    {
                        diceRoll = Configuration.RNG.Next(1, HitDiceQuality + 1);
                        total = diceRoll + RefChar.ConstitutionModifier;
                        RefChar.CurrentHealth += total;
                        rolls.Add(diceRoll);
                        modTotal += RefChar.ConstitutionModifier;
                        healTotal += total;
                        CurrentHitDice--;
                    }
                    while (Convert.ToDouble(RefChar.CurrentHealth) / Convert.ToDouble(RefChar.MaxHealth) < 0.50 && CurrentHitDice > 0);
                    message = RefChar.Name + " rolled hit dice to Bruised.";
                    message += "\nResult: " + healTotal + " hit point" + ((healTotal > 1) ? "s" : "");
                    message += Configuration.MainModelRef.SettingsView.ShowDiceRolls ? "\nRoll: [" + HelperMethods.GetStringFromList(rolls, " + ") + "] + " + modTotal : "";
                    break;
                case "ToFine":
                    if (RefChar.Status == "Fine")
                    {
                        HelperMethods.AddToPlayerLog("Character is already at Fine or better.");
                        return;
                    }
                    do
                    {
                        diceRoll = Configuration.RNG.Next(1, HitDiceQuality + 1);
                        total = diceRoll + RefChar.ConstitutionModifier;
                        RefChar.CurrentHealth += total;
                        rolls.Add(diceRoll);
                        modTotal += RefChar.ConstitutionModifier;
                        healTotal += total;
                        CurrentHitDice--;
                    }
                    while (Convert.ToDouble(RefChar.CurrentHealth) / Convert.ToDouble(RefChar.MaxHealth) < 0.75 && CurrentHitDice > 0);
                    message = RefChar.Name + " rolled hit dice to Fine.";
                    message += "\nResult: " + healTotal + " hit point" + ((healTotal > 1) ? "s" : "");
                    message += Configuration.MainModelRef.SettingsView.ShowDiceRolls ? "\nRoll: [" + HelperMethods.GetStringFromList(rolls, " + ") + "] + " + modTotal : "";
                    break;
                case "ToNearMax":
                    int maxRoll = HitDiceQuality + RefChar.ConstitutionModifier;
                    if (RefChar.MaxHealth - RefChar.CurrentHealth < maxRoll)
                    {
                        HelperMethods.AddToPlayerLog("Character is already within one hit dice of maximum health.");
                        return;
                    }
                    do
                    {
                        diceRoll = Configuration.RNG.Next(1, HitDiceQuality + 1);
                        total = diceRoll + RefChar.ConstitutionModifier;
                        RefChar.CurrentHealth += total;
                        rolls.Add(diceRoll);
                        modTotal += RefChar.ConstitutionModifier;
                        healTotal += total;
                        CurrentHitDice--;
                    }
                    while (RefChar.MaxHealth - RefChar.CurrentHealth >= maxRoll && CurrentHitDice > 0);
                    message = RefChar.Name + " rolled hit dice to near max.";
                    message += "\nResult: " + healTotal + " hit point" + ((healTotal > 1) ? "s" : "");
                    message += Configuration.MainModelRef.SettingsView.ShowDiceRolls ? "\nRoll: [" + HelperMethods.GetStringFromList(rolls, " + ") + "] + " + modTotal : "";
                    break;
                default:
                    HelperMethods.WriteToLogFile("Invalid parameter " + diceLimit + " passed to HitDiceSet.RollHitDice", true);
                    return;
            }
            HelperMethods.AddToPlayerLog(message, "Default", true);
        }
        #endregion
        #region RemoveHitDice
        private RelayCommand _RemoveHitDice;
        public ICommand RemoveHitDice
        {
            get
            {
                if (_RemoveHitDice == null)
                {
                    _RemoveHitDice = new RelayCommand(param => DoRemoveHitDice());
                }
                return _RemoveHitDice;
            }
        }
        private void DoRemoveHitDice()
        {
            Configuration.MainModelRef.CharacterBuilderView.ActiveCharacter.HitDiceSets.Remove(this);
        }
        #endregion

    }
}
