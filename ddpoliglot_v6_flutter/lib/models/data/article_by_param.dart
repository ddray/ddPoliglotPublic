import 'package:json_annotation/json_annotation.dart';

part 'article_by_param.g.dart';

@JsonSerializable()
class ArticleByParam {
  final int articleByParamID;
  final String? userID;
  final int nativeLanguageID;
  final int learnLanguageID;
  final int type; // 0 - article by vocabulary, 1 - article by dialog
  final String name;
  final String dataJson;
  final bool isTemplate;
  final bool isShared;

  ArticleByParam(
    this.articleByParamID,
    this.userID,
    this.nativeLanguageID,
    this.learnLanguageID,
    this.type, // 0 - article by vocabulary, 1 - article by dialog
    this.name,
    this.dataJson,
    this.isTemplate,
    this.isShared,
  );

  factory ArticleByParam.fromJson(Map<String, dynamic> data) =>
      _$ArticleByParamFromJson(data);

  Map<String, dynamic> toJson() => _$ArticleByParamToJson(this);

  ArticleByParam clone() {
    return ArticleByParam(
      articleByParamID,
      userID,
      nativeLanguageID,
      learnLanguageID,
      type, // 0 - article by vocabulary, 1 - article by dialog
      name,
      dataJson,
      isTemplate,
      isShared,
    );
  }
}
