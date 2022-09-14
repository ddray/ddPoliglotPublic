export interface IPhraseHiddenProperty {
    Pron?: string;
}

export interface ITextWithPhraseHiddenProperty {
    Text: string;
    PhraseHiddenProperty?: IPhraseHiddenProperty;
}
