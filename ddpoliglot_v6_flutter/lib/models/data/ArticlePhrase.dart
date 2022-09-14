// ignore_for_file: file_names, annotate_overrides, hash_and_equals
import '../play_list_item.dart';
import './ArticleActor.dart';
import './Article.dart';

import 'package:json_annotation/json_annotation.dart';

part 'ArticlePhrase.g.dart';

@JsonSerializable()
class ArticlePhrase {
  int articlePhraseID;
  String? keyString;
  int articleID;
  Article? article;
  String text;
  int hashCode;
  String? textSpeechFileName;
  int speachDuration;
  int pause; // how many additional seconds add after source audio
  int articleActorID;
  ArticleActor? articleActor;
  String? trText;
  int trHashCode;
  String? trTextSpeechFileName;
  int trSpeachDuration;
  int trPause; // how many additional seconds add after translation audio
  int trArticleActorID;
  ArticleActor? trArticleActor;

  // props
  int orderNum;
  int activityType; // phrase or translation is used in article, 0 = source first, 1 = translation first
  int type; // 0 - text speach, 1 - Dictor Speach

  // grouping as child
  String? parentKeyString;
  String? childrenType; // 0 01 012 0123, if is empty - added by hand

  // as parent
  bool hasChildren;
  bool childrenClosed;

  bool selected;

  bool silent; // don't reproduce this phrase

  @JsonKey(ignore: true)
  PlayListItem? playListItem; // for sPlayer

  ArticlePhrase(
    this.articlePhraseID,
    this.keyString,
    this.articleID,
    this.article,
    this.text,
    this.hashCode,
    this.textSpeechFileName,
    this.speachDuration,
    this.pause, // how many additional seconds add after source audio
    this.articleActorID,
    this.articleActor,
    this.trText,
    this.trHashCode,
    this.trTextSpeechFileName,
    this.trSpeachDuration,
    this.trPause, // how many additional seconds add after translation audio
    this.trArticleActorID,
    this.trArticleActor,

    // props
    this.orderNum,
    this.activityType, // phrase or translation is used in article, 0 = source first, 1 = translation first
    this.type, // 0 - text speach, 1 - Dictor Speach

    // grouping as child
    this.parentKeyString,
    this.childrenType, // 0 01 012 0123, if is empty - added by hand

    // as parent
    this.hasChildren,
    this.childrenClosed,
    this.selected,
    this.silent, // don't reproduce this phrase
  );

  factory ArticlePhrase.fromJson(Map<String, dynamic> data) =>
      _$ArticlePhraseFromJson(data);

  Map<String, dynamic> toJson() => _$ArticlePhraseToJson(this);
}
