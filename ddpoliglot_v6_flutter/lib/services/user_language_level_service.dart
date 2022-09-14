import '../models/data/user_language_level.dart';
import '../utils/http_utils.dart';

class UserLanguageLevelService {
  static Future<UserLanguageLevel?> get() async {
    final extractedData = await HttpUtils.get('UserLanguageLevel/Get1');
    var result = extractedData == null
        ? null
        : UserLanguageLevel.fromJson(extractedData as Map<String, dynamic>);
    return result;
  }

  static Future<void> setByLevel(int level) async {
    await HttpUtils.post('UserLanguageLevel/SetByLevel1', '$level');
  }
}
