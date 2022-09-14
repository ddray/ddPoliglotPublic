// ignore_for_file: file_names

import './ArticlePhrase.dart';
import './ArticleActor.dart';

import 'package:json_annotation/json_annotation.dart';

part 'Article.g.dart';

@JsonSerializable()
class Article {
  int articleID;
  String? userID;
  String? name;
  String? language;
  String? languageTranslation;
  String? textHashCode;
  String? textSpeechFileName;
  String? videoFileName;
  List<ArticlePhrase> articlePhrases;
  List<ArticleActor> articleActors;

  Article(
    this.articleID,
    this.userID,
    this.name,
    this.language,
    this.languageTranslation,
    this.textHashCode,
    this.textSpeechFileName,
    this.videoFileName,
    this.articlePhrases,
    this.articleActors,
  );

  factory Article.fromJson(Map<String, dynamic> data) =>
      _$ArticleFromJson(data);

  Map<String, dynamic> toJson() => _$ArticleToJson(this);
}
