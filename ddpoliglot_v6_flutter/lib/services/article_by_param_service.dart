import '../utils/http_utils.dart';
import '../models/article_by_param_data.dart';

class ArticleByParamService {
  // static Future<ArticleByParamAndDataWithAudioData>
  //     getByLessonTypeWithReadyAudio(String? token, int lessonType) async {
  //   try {
  //     final extractedData = await HttpUtils.get(
  //         'ArticleByParam/getByLessonTypeWithReadyAudio?lessonType=$lessonType');

  //     final articleByParamDataWithAudio = ArticleByParamData.fromJson(
  //         extractedData['articleByParamDataWithAudio']);

  //     // final articleByParam = ArticleByParam.fromJson(extractedData['data']);

  //     final jsonS = json.encode(extractedData['articleByParamDataWithAudio']);
  //     final l = jsonS.length;

  //     final jj = json.encode(
  //         extractedData['articleByParamDataWithAudio']['mixParamsList'][0]);
  //     final ll = jj.length;
  //     // final jsonS2 = json.encode(extractedData['data']);
  //     // final l2 = jsonS.length;

  //     return ArticleByParamAndDataWithAudioData(
  //         //articleByParam,
  //         articleByParamDataWithAudio);
  //   } catch (error, stackTrace) {
  //     Catcher.reportCheckedError(error, stackTrace);
  //     rethrow;
  //   }
  // }

  static Future<SchemaResponse> getSchemas(curVersion) async {
    final extractedData =
        await HttpUtils.get('ArticleByParam/GetSchemas?version=$curVersion');
    var serverVersion = extractedData["version"];
    var schemas = (extractedData["schemas"] as List<dynamic>)
        .map((e) => ArticleByParamData.fromJson(e as Map<String, dynamic>))
        .toList();
    return SchemaResponse(serverVersion, schemas);
  }
}

class SchemaResponse {
  final int version;
  final List<ArticleByParamData> schemas;
  SchemaResponse(this.version, this.schemas);
}
