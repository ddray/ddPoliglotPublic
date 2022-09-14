// ignore_for_file: file_names, hash_and_equals, annotate_overrides
import '../player_classes.dart';
import './Language.dart';
import './WordPhraseWord.dart';
import './WordTranslation.dart';
import './WordUser.dart';
import './WordPhrase.dart';

import 'package:json_annotation/json_annotation.dart';
part 'Word.g.dart';

@JsonSerializable()
class Word extends Plailable {
  int wordID;
  int languageID;
  Language? language;
  String text;
  int rate;
  String? pronunciation;
  int hashCode;
  String? textSpeechFileName;
  int speachDuration;
  int hashCodeSpeed1;
  String? textSpeechFileNameSpeed1;
  int speachDurationSpeed1;
  int hashCodeSpeed2;
  String? textSpeechFileNameSpeed2;
  int speachDurationSpeed2;
  List<WordPhraseWord>? wordPhraseWords;
  WordTranslation? wordTranslation;
  WordUser? wordUser;
  List<WordPhrase>? wordPhraseSelected;
  int oxfordLevel;

  @JsonKey(ignore: true)
  bool selected = false;

  @JsonKey(ignore: true)
  bool loading = false;

  @JsonKey(ignore: true)
  int wordLessonType = 0; // type from user lesson. main or repetition

  Word(
    this.wordID,
    this.languageID,
    this.language,
    this.text,
    this.rate,
    this.pronunciation,
    this.hashCode,
    this.textSpeechFileName,
    this.speachDuration,
    this.hashCodeSpeed1,
    this.textSpeechFileNameSpeed1,
    this.speachDurationSpeed1,
    this.hashCodeSpeed2,
    this.textSpeechFileNameSpeed2,
    this.speachDurationSpeed2,
    this.wordPhraseWords,
    this.wordTranslation,
    this.wordUser,
    this.wordPhraseSelected,
    this.oxfordLevel,
  );

  factory Word.fromJson(Map<String, dynamic> data) => _$WordFromJson(data);

  Map<String, dynamic> toJson() => _$WordToJson(this);
}
