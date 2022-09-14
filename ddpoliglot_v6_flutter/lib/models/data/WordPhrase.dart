// ignore_for_file: file_names, hash_and_equals, annotate_overrides
import '../player_classes.dart';
import './Language.dart';
import './WordPhraseWord.dart';
import './WordPhraseTranslation.dart';

import 'package:json_annotation/json_annotation.dart';

part 'WordPhrase.g.dart';

@JsonSerializable()
class WordPhrase extends Plailable {
  int wordPhraseID;
  int languageID;
  Language? language;
  String text;
  int hashCode;
  String? textSpeechFileName;
  int speachDuration;
  int hashCodeSpeed1;
  String? textSpeechFileNameSpeed1;
  int speachDurationSpeed1;
  int hashCodeSpeed2;
  String? textSpeechFileNameSpeed2;
  int speachDurationSpeed2;
  int sourceType; // 0 - dict, 1 - api, 2 - by hand
  String? wordsUsed;
  List<WordPhraseWord> wordPhraseWords;
  WordPhraseTranslation? wordPhraseTranslation;
  int phraseOrderInCurrentWord;
  int currentWordID;

  WordPhrase(
    this.wordPhraseID,
    this.languageID,
    this.language,
    this.text,
    this.hashCode,
    this.textSpeechFileName,
    this.speachDuration,
    this.hashCodeSpeed1,
    this.textSpeechFileNameSpeed1,
    this.speachDurationSpeed1,
    this.hashCodeSpeed2,
    this.textSpeechFileNameSpeed2,
    this.speachDurationSpeed2,
    this.sourceType, // 0 - dict, 1 - api, 2 - by hand
    this.wordsUsed,
    this.wordPhraseWords,
    this.wordPhraseTranslation,
    this.phraseOrderInCurrentWord,
    this.currentWordID,
  );

  factory WordPhrase.fromJson(Map<String, dynamic> data) =>
      _$WordPhraseFromJson(data);

  Map<String, dynamic> toJson() => _$WordPhraseToJson(this);
}
