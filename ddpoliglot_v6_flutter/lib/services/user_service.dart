// import 'package:flutter/material.dart';

// import '../utils/http_utils.dart';

// class UserService {
//   static Future<Object> getUserResultsData() async {
//     try {
//       final extractedData = await HttpUtils.get('User/GetUserResultsData');
//       var result = extractedData == null
//           ? null
//           : null; //UserLanguageLevel.fromJson(extractedData as Map<String, dynamic>);
//       debugPrint('getUserResultsData: $result');
//       return result;
//     } catch (ex) {
//       debugPrint('getUserResultsData: ${ex.toString()}');
//       rethrow;
//     }
//   }
// }
