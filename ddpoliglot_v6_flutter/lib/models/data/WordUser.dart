// ignore_for_file: file_names
import 'Word.dart';
import './Language.dart';

import 'package:json_annotation/json_annotation.dart';

part 'WordUser.g.dart';

@JsonSerializable()
class WordUser {
  int wordUserID;
  int wordID;
  Word? word;
  String userID;
  int languageID;
  Language? language;
  int grade;
  int lastRepeatInArticleByParamID;
  int lastRepeatInLessonNum;
  int lastRepeatWordPhraseId;
  int sourceType; // 0 - set grade by hand, 1- by auto (language level)

  WordUser(
      this.wordUserID,
      this.wordID,
      this.word,
      this.userID,
      this.languageID,
      this.language,
      this.grade,
      this.lastRepeatInArticleByParamID,
      this.lastRepeatInLessonNum,
      this.lastRepeatWordPhraseId,
      this.sourceType);

  factory WordUser.fromJson(Map<String, dynamic> data) =>
      _$WordUserFromJson(data);

  Map<String, dynamic> toJson() => _$WordUserToJson(this);
}
