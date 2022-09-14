import '../models/data/Language.dart';

class LanguageService {
  static Future<List<Language>> getAll() async {
    return Future.delayed(const Duration(seconds: 0), () {
      List<Language> items = [];
      items.add(Language(1, "en", "en-US", "English(US, GB)", "en"));
      items.add(Language(2, "ru", "ru-RU", "Russian", "ru"));
      return items;
    });
  }
}
