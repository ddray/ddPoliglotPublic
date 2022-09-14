// ignore_for_file: file_names
import 'package:ddpoliglot_v6_flutter/models/dict_word.dart';
import 'package:json_annotation/json_annotation.dart';

import 'Word.dart';

part 'user_lesson_word.g.dart';

@JsonSerializable()
class UserLessonWord {
  int userLessonWordID;
  int userLessonID;
  int wordID;
  Word? word;
  int wordType;
  DictWord? dictWord;

  UserLessonWord(this.userLessonWordID, this.userLessonID, this.wordID,
      this.word, this.wordType);

  factory UserLessonWord.fromJson(Map<String, dynamic> data) =>
      _$UserLessonWordFromJson(data);

  Map<String, dynamic> toJson() => _$UserLessonWordToJson(this);
}
