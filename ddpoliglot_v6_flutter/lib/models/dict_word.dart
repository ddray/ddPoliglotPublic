// ignore_for_file: file_names, hash_and_equals, annotate_overrides

import 'package:ddpoliglot_v6_flutter/models/data/Word.dart';
import 'package:json_annotation/json_annotation.dart';
part 'dict_word.g.dart';

@JsonSerializable()
class DictWord {
  int wordID;
  String text;
  int rate;
  String? pronunciation;
  String? translation;
  int oxfordLevel;
  int wordUserID;
  int grade;
  int lastRepeatInArticleByParamID;
  int lastRepeatInLessonNum;
  int lastRepeatWordPhraseId;
  int sourceType; // 0 - set grade by hand, 1- by auto (language level)
  String textClear;
  int readyForLesson;

  @JsonKey(ignore: true)
  bool selected = false;

  @JsonKey(ignore: true)
  bool loading = false;

  @JsonKey(ignore: true)
  int wordLessonType = 0; // type from user lesson. main or repetition

  @JsonKey(ignore: true)
  bool fromSearch = false; // type from user lesson. main or repetition

  DictWord(
    this.wordID,
    this.text,
    this.rate,
    this.pronunciation,
    this.translation,
    this.grade,
    this.oxfordLevel,
    this.wordUserID,
    this.lastRepeatInArticleByParamID,
    this.lastRepeatInLessonNum,
    this.lastRepeatWordPhraseId,
    this.sourceType,
    this.textClear,
    this.readyForLesson,
  );

  setWordUserData(Word w) {
    grade = w.wordUser?.grade ?? 0;
    wordUserID = w.wordUser?.wordUserID ?? 0;
    lastRepeatInArticleByParamID =
        w.wordUser?.lastRepeatInArticleByParamID ?? 0;
    lastRepeatInLessonNum = w.wordUser?.lastRepeatInLessonNum ?? 0;
    lastRepeatWordPhraseId = w.wordUser?.lastRepeatWordPhraseId ?? 0;
    sourceType = w.wordUser?.sourceType ?? 0;
  }

  factory DictWord.fromWord(Word w) => DictWord(
        w.wordID,
        w.text,
        w.rate,
        w.pronunciation,
        w.wordTranslation?.text ?? '',
        w.wordUser?.grade ?? 0,
        w.oxfordLevel,
        w.wordUser?.wordUserID ?? 0,
        w.wordUser?.lastRepeatInArticleByParamID ?? 0,
        w.wordUser?.lastRepeatInLessonNum ?? 0,
        w.wordUser?.lastRepeatWordPhraseId ?? 0,
        w.wordUser?.sourceType ?? 0,
        w.text,
        1,
      );

  factory DictWord.fromJson(Map<String, dynamic> data) =>
      _$DictWordFromJson(data);

  Map<String, dynamic> toJson() => _$DictWordToJson(this);
}
