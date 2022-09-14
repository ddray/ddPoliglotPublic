// ignore_for_file: file_names, hash_and_equals, annotate_overrides
import '../player_classes.dart';
import 'Word.dart';
import './Language.dart';

import 'package:json_annotation/json_annotation.dart';

part 'WordTranslation.g.dart';

@JsonSerializable()
class WordTranslation extends Plailable {
  int wordTranslationID;
  int wordID;
  Word? word;
  int languageID;
  Language? language;
  String text;
  int hashCode;
  String? textSpeechFileName;
  int speachDuration;
  int readyForLessonPhrasiesCnt; // phrases for this word, ordered, translated and speeched for this language

  WordTranslation(
    this.wordTranslationID,
    this.wordID,
    this.word,
    this.languageID,
    this.language,
    this.text,
    this.hashCode,
    this.textSpeechFileName,
    this.speachDuration,
    this.readyForLessonPhrasiesCnt, // phrases for this word, ordered, translated and speeched for this language
  );

  factory WordTranslation.fromJson(Map<String, dynamic> data) =>
      _$WordTranslationFromJson(data);

  Map<String, dynamic> toJson() => _$WordTranslationToJson(this);
}
