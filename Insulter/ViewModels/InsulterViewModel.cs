﻿using Insulter.Models;

namespace Insulter.ViewModels;

public partial class InsulterViewModel : TextToSpeechViewModel 
{

    private const string WELCOME_MESSAGE = "Salutations! Prithee selectest thou the Shakespearean insult thou wouldst hear me utter.";

    /// <summary>
    /// counter for number of insults spoken
    /// </summary>
    private int _insultsSpoken = 0;
    public int InsultsSpoken
    {
        get => _insultsSpoken;
        private set => SetProperty(ref _insultsSpoken, value);

	} //InsultsSpoken

	/// <summary>
	/// insults list
	/// </summary>
	private List<string> _insultsList = [];
    public List<string> InsultsList
    {
        get => _insultsList; 
        private set => SetProperty(ref _insultsList, value);

	} //InsultsList


	/// <summary>
	/// Creates and initializes new InsulterViewModel object
	/// </summary>
	public InsulterViewModel()
    {
        InsultsList = new Insults().InsultsList;
        Initialized &= InsultsList.Count > 0;
        InsultsList.Insert(0, WELCOME_MESSAGE);

    } //InsulterViewModel


    /// <summary>
    /// Speaks insult from insult list at specified positio 
    /// </summary>
    /// <param name="insultIndex">Integer index of insult in InsultsList to speak</param>
    public void SpeakInsult(int insultIndex)
    {
        if (InsultsList.Count == 0 || insultIndex < 0 || insultIndex > InsultsList.Count - 1)
        {
            throw new ArgumentException($"no insult at specified index {insultIndex}, index value is invalid");
        }
        TextToSpeak = InsultsList[insultIndex];
        DeleteInsultAt(insultIndex);
        InsultsSpoken++;
        SpeakNowAsync();

    } //SpeakInsult


    /// <summary>
    /// Removes insult from list at specified index and re-initializes insults list if empty
    /// </summary>
    /// <param name="insultIndex">Integer list index of insult to be removed</param>
    private void DeleteInsultAt(int insultIndex)
    {
        if (InsultsList.Count == 0 || insultIndex < 0 || insultIndex > InsultsList.Count - 1)
        {
            throw new ArgumentException($"no insult at specified index {insultIndex}, index value is invalid");
        }

        InsultsList.RemoveAt(insultIndex);

        //reinitialize insults list if last insult was just removed
        if (0 == InsultsList.Count)
        {
			_insultsList = new Insults().InsultsList;
		}

    } //DeleteInsultAt



} //InsulterViewModel