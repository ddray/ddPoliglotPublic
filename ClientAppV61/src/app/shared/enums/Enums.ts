export enum ArticleActorRole {
  textSpeaker = 0,
  textTranslatedSpeaker = 1,
  textDictorSpeaker = 2,
  textTranslatedDictorSpeaker = 3,
}

export enum ArticlePhraseType {
  speaker = 0,
  dictor = 1,
}

export enum ArticlePhraseActivityType {
  textFirst = 0,
  trTextFirst = 1,
}

export enum PhrasesMixType {
  wave = 0,
  random = 1
}

export enum PhrasesSplitType {
  space = 0,
  dot = 1,
  NL = 2,
}
