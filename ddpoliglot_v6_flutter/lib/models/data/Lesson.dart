// ignore_for_file: file_names
import './Language.dart';
import 'article_by_param.dart';

import 'package:json_annotation/json_annotation.dart';

part 'Lesson.g.dart';

@JsonSerializable()
class Lesson {
  int lessonID;
  int parentID; // Parent Lesson
  int languageID;
  Language language; // language to learn
  int nativeLanguageID; // native user language
  String name;
  String title;
  String description;
  String content;
  int order;
  String video1;
  String video2;
  String video3;
  String video4;
  String video5;
  String audio1;
  String audio2;
  String audio3;
  String audio4;
  String audio5;
  String description1;
  String description2;
  String description3;
  String description4;
  String description5;
  DateTime modified;
  String state;
  String pageName;
  int articleByParamID;
  ArticleByParam articleByParam;

  Lesson(
    this.lessonID,
    this.parentID, // Parent Lesson
    this.languageID,
    this.language, // language to learn
    this.nativeLanguageID, // native user language
    this.name,
    this.title,
    this.description,
    this.content,
    this.order,
    this.video1,
    this.video2,
    this.video3,
    this.video4,
    this.video5,
    this.audio1,
    this.audio2,
    this.audio3,
    this.audio4,
    this.audio5,
    this.description1,
    this.description2,
    this.description3,
    this.description4,
    this.description5,
    this.modified,
    this.state,
    this.pageName,
    this.articleByParamID,
    this.articleByParam,
  );

  factory Lesson.fromJson(Map<String, dynamic> data) => _$LessonFromJson(data);

  Map<String, dynamic> toJson() => _$LessonToJson(this);
}
