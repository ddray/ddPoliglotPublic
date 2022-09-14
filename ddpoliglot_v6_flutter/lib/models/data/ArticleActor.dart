// ignore_for_file: file_names
import './Article.dart';

import 'package:json_annotation/json_annotation.dart';

part 'ArticleActor.g.dart';

@JsonSerializable()
class ArticleActor {
  int articleActorID;
  String? keyString;
  String? name;
  int role; // 0 text speach, 1- dictor voice
  bool defaultInRole;
  String? language;
  String? voiceName;
  double voiceSpeed;
  double voicePitch;
  int articleID;
  Article? article;

  ArticleActor(
    this.articleActorID,
    this.keyString,
    this.name,
    this.role, // 0 text speach, 1- dictor voice
    this.defaultInRole,
    this.language,
    this.voiceName,
    this.voiceSpeed,
    this.voicePitch,
    this.articleID,
    this.article,
  );

  factory ArticleActor.fromJson(Map<String, dynamic> data) =>
      _$ArticleActorFromJson(data);

  Map<String, dynamic> toJson() => _$ArticleActorToJson(this);
}
