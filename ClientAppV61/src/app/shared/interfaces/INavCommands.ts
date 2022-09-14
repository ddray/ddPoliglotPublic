export interface INavCommandInput {
  command: navCommandsInput,
  payload?: any
}

export interface INavCommandOutput {
  command: navCommandsOutput,
  payload?: any
}

export enum navCommandsInput {
  Article_All_Disabled = 0,
  Article_Save_Disabled = 1,
  Article_ShowPhraseToolbox_Disabled = 2,
  Article_MakeAudioFile_Disabled = 3,
  Article_Save_Loader = 4,
  Article_ShowPhraseToolbox_Loader = 5,
  Article_MakeAudioFile_Loader = 6,
  Article_AudioFile_Value = 7,
  Article_RecalculatePauses_Loader = 8,
  Article_MakeVideoFile_Disabled = 9,
  Article_MakeVideoFile_Loader = 10,
  Article_VideoFile_Value = 11,
}

export enum navCommandsOutput {
  Articles_Add_Click,
  Article_Save_Click,
  Article_ShowPhraseToolbox_Click,
  Article_MakeAudioFile_Click,
  Article_RecalculatePauses_Click,
  Article_CollapseAll_Click,
  Article_MakeVideoFile_Click,
  Article_OpenVideoDialog_Click
}
