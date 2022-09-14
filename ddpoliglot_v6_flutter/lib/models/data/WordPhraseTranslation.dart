// ignore_for_file: file_names, hash_and_equals, annotate_overrides
import '../player_classes.dart';
import './Language.dart';
import './WordPhrase.dart';

import 'package:json_annotation/json_annotation.dart';

part 'WordPhraseTranslation.g.dart';

@JsonSerializable()
class WordPhraseTranslation extends Plailable {
  int wordPhraseTranslationID;
  int wordPhraseID;
  WordPhrase? wordPhrase;
  int languageID;
  Language? language;
  String text;
  int hashCode;
  String? textSpeechFileName;
  int speachDuration;

  WordPhraseTranslation(
    this.wordPhraseTranslationID,
    this.wordPhraseID,
    this.wordPhrase,
    this.languageID,
    this.language,
    this.text,
    this.hashCode,
    this.textSpeechFileName,
    this.speachDuration,
  );

  factory WordPhraseTranslation.fromJson(Map<String, dynamic> data) =>
      _$WordPhraseTranslationFromJson(data);

  Map<String, dynamic> toJson() => _$WordPhraseTranslationToJson(this);
}
