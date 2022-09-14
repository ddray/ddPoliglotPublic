// ignore_for_file: file_names
// import 'package:flutter/material.dart';
// import 'data/article_by_param.dart';
import 'article_by_param_data.dart';

class ArticleByParamAndDataWithAudioData {
  // ArticleByParam data;
  ArticleByParamData articleByParamDataWithAudio;

  ArticleByParamAndDataWithAudioData(
      //this.data,
      this.articleByParamDataWithAudio);
}

// class ArticleByParamAndDataWithAudio with ChangeNotifier {
//   ArticleByParam? _articleByParam;
//   ArticleByParamData? _articleByParamDataWithAudio;
//   final String? authToken;
//   final String? userId;

//   ArticleByParamAndDataWithAudio(this.authToken, this.userId,
//       this._articleByParam, this._articleByParamDataWithAudio);

//   // ArticleByParam? get articleByParam {
//   //   if (_articleByParam != null) {
//   //     return _articleByParam!.clone();
//   //   }

//   //   return null;
//   // }

//   // ArticleByParamData? get articleByParamDataWithAudio {
//   //   if (_articleByParamDataWithAudio != null) {
//   //     return _articleByParamDataWithAudio!.clone();
//   //   }

//   //   return null;
//   // }

//   // Future<void> fetchAndSet(int id) async {
//   //   try {
//   //     var url = Uri.parse(
//   //         'https://10.0.2.2:44342/api/ArticleByParam/GetByIdWithReadyAudio1?id=$id');

//   //     final headers = {
//   //       'Content-Type': 'application/json',
//   //       'Authorization': 'Bearer $authToken',
//   //     };

//   //     final response = await http.get(url, headers: headers);

//   //     final extractedData = json.decode(response.body);
//   //     if (extractedData == null) {
//   //       return;
//   //     }

//   //     _articleByParam = ArticleByParam.fromJson(extractedData['data']);
//   //     _articleByParamDataWithAudio = ArticleByParamData.fromJson(
//   //         extractedData['articleByParamDataWithAudio']);

//   //     debugPrint("articleByParamDataWithAudio: downloaded");

//   //     notifyListeners();
//   //   } catch (error) {
//   //     debugPrint('error:');
//   //     debugPrint(error.toString());
//   //     rethrow;
//   //   }
//   // }
// }
