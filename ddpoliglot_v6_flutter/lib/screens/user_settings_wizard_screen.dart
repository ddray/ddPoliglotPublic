// ignore_for_file: unnecessary_const

import 'package:auto_size_text/auto_size_text.dart';
import 'package:ddpoliglot_v6_flutter/models/user_settings_state_data.dart';
import 'package:ddpoliglot_v6_flutter/models/data/Language.dart';
import 'package:ddpoliglot_v6_flutter/models/data/user_language_level.dart';
import 'package:ddpoliglot_v6_flutter/models/dict_word.dart';
import 'package:ddpoliglot_v6_flutter/services/language_service.dart';
import 'package:ddpoliglot_v6_flutter/services/user_language_level_service.dart';
import 'package:ddpoliglot_v6_flutter/services/word_service.dart';
import 'package:ddpoliglot_v6_flutter/utils/colors_utils.dart';
import 'package:ddpoliglot_v6_flutter/utils/utils.dart';
import 'package:ddpoliglot_v6_flutter/widgets/grade_widget.dart';
import 'package:ddpoliglot_v6_flutter/widgets/quest_button_widget.dart';
import 'package:ddpoliglot_v6_flutter/widgets/quest_screen_widget.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:collection/collection.dart';

import '../exeptions/custom_exception.dart';
import '../providers/auth/auth_provider.dart';
import '../providers/user_settings/user_settings_provider.dart';

class UserSettingsWizardScreen extends StatefulWidget {
  const UserSettingsWizardScreen({Key? key}) : super(key: key);

  @override
  UserSettingsWizardScreenState createState() =>
      UserSettingsWizardScreenState();
}

class UserSettingsWizardScreenState extends State<UserSettingsWizardScreen> {
  UserSettingsState _appSettings = UserSettingsState.initial();
  late List<Language> _languages;
  late bool _isLoading = true;
  int? languageLevelWizardStep;
  List<DictWord> _words = [];

  bool _isInit = false;

  @override
  void didChangeDependencies() {
    if (!_isInit) {
      _isInit = true;
      _isLoading = true;
      LanguageService.getAll().then((languagesResult) {
        setState(() {
          _languages = languagesResult;
          _words = WordService.getTestWords();
          _isLoading = false;
        });
      });
    }

    super.didChangeDependencies();
  }

  Future<void> save(int userLanguageLevel) async {
    setState(() {
      _isLoading = true;
    });

    final userSettingsProvider = context.read<UserSettingsProvider>();
    _appSettings = _appSettings.copyWith(
        userLanguageLevel: UserLanguageLevel(
            0,
            _appSettings.learnLanguage!.languageID,
            userSettingsProvider.authState.userId,
            userLanguageLevel));

    var errorMessage = '';
    try {
      await userSettingsProvider.update(_appSettings, notify: false);
      await UserLanguageLevelService.setByLevel(userLanguageLevel);
      await userSettingsProvider.update(_appSettings, notify: true);
      // provider will set new page
    } on CustomException catch (error) {
      errorMessage = error.message;
    } catch (error) {
      errorMessage = 'Упс..., произошла ошибка. Попробуйте еще раз';
      rethrow;
    } finally {
      if (errorMessage.isNotEmpty) {
        Utils.showErrorDialog(errorMessage, context);
      }
      setState(() {
        _isLoading = false;
      });
    }
  }

  Future<void> saveSettingsOnly() async {
    setState(() {
      _isLoading = true;
    });

    final userSettingsProvider =
        Provider.of<UserSettingsProvider>(context, listen: false);
    await userSettingsProvider.update(_appSettings, notify: true);
    // provider will set new page
  }

  Widget _scfLoading() {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Настройки, Загрузка данных'),
      ),
      body: const Center(child: CircularProgressIndicator()),
    );
  }

  Widget _scfSelectNativeLanguage() {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Родной язык'),
      ),
      body: QuestScreenWidget(
        question: 'Мой родной язык',
        answers: [
          QuestButtonWidget(
            'Русский.',
            () {
              setState(() {
                _appSettings = _appSettings.copyWith(
                    nativeLanguage:
                        _languages.firstWhere((x) => x.languageID == 2));
              });
            },
            assetImageName: 'russia_round_icon_64.png',
          ),
          QuestButtonWidget(
            'Slovakian.',
            () {
              // setState(() {
              //   _appSettings = _appSettings.copyWith(
              //       nativeLanguage:
              //           _languages.firstWhere((x) => x.languageID == 4));
              // });
            },
            assetImageName: 'slovakia_round_icon_64.png',
          ),
        ],
      ),
    );
  }

  Widget _scfSelectLearnLanguage() {
    final auth = Provider.of<AuthProvider>(context, listen: false);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Учу язык'),
      ),
      body: QuestScreenWidget(
        question: 'Хочу учить язык',
        answers: [
          QuestButtonWidget(
            'Английский.',
            () {
              var languageID = 1;
              var userlanguageLevel = auth.state.userLanguageLevels
                  .firstWhereOrNull((x) => x.languageID == languageID);
              if (userlanguageLevel == null) {
                setState(() {
                  _appSettings = _appSettings.copyWith(
                      learnLanguage: _languages
                          .firstWhere((x) => x.languageID == languageID));
                });
              } else {
                // already saved in auth
                _appSettings = _appSettings.copyWith(
                    learnLanguage: _languages
                        .firstWhere((x) => x.languageID == languageID),
                    userLanguageLevel: userlanguageLevel);
                languageLevelWizardStep = 1;
                saveSettingsOnly();
              }
            },
            assetImageName: 'united_kingdom_round_icon_64.png',
          ),
          QuestButtonWidget(
            'Slovakian.',
            () {
              // setState(() {
              //   _appSettings = _appSettings.copyWith(
              //       learnLanguage: _languages.firstWhere((x) => x.languageID == 4));
              // });
            },
            assetImageName: 'slovakia_round_icon_64.png',
          ),
        ],
      ),
    );
  }

  Widget _scfSelectLanguageLevel() {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Уровень знания языка'),
      ),
      body: QuestScreenWidget(
        question: 'Ваш уровень знания языка',
        answers: [
          QuestButtonWidget(
            'Определить мой уровень.',
            () {
              setState(() {
                languageLevelWizardStep = 0;
              });
            },
            subtitle:
                'Пройти короткий тест чтобы настроить систему на ваш уровень знаний.',
          ),
          QuestButtonWidget(
            'Буду учить с самого начала.',
            () async {
              await save(0);
            },
            subtitle: 'Только начал учить, знаю только пару слов.',
          ),
        ],
      ),
    );
  }

  Widget _scfSelectLanguageLevelWizard() {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Определить уровень языка.'),
      ),
      body: CustomScrollView(
        key: const ValueKey('usw_a1'),
        slivers: <Widget>[
          SliverAppBar(
            toolbarHeight: 150,
            backgroundColor: Colors.white,
            title: Container(
              height: 150,
              alignment: Alignment.center,
              width: double.infinity,
              padding: const EdgeInsets.all(5),
              child: ListView(
                children: [
                  const SizedBox(
                    height: 20,
                  ),
                  AutoSizeText(
                    'Дайте статус каждому слову в списке.',
                    style: TextStyle(
                        fontSize: 20,
                        color: ColorsUtils.customBlackColor,
                        fontWeight: FontWeight.bold),
                    textAlign: TextAlign.center,
                    maxLines: 1,
                  ),
                  const SizedBox(
                    height: 10,
                  ),
                  Container(
                    alignment: Alignment.centerRight,
                    child: Container(
                        padding: const EdgeInsets.all(8),
                        child: Row(
                          mainAxisAlignment: MainAxisAlignment.spaceAround,
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            Column(
                              children: [
                                Row(
                                  children: [
                                    AutoSizeText(
                                      'Слово знаю, учить не надо.',
                                      style: TextStyle(
                                          fontSize: 15,
                                          color: ColorsUtils.customBlackColor),
                                      maxLines: 1,
                                    ),
                                    const SizedBox(
                                      width: 10,
                                    ),
                                    Icon(
                                      Icons.sentiment_satisfied_alt,
                                      color: Colors.amber[800],
                                    ),
                                  ],
                                ),
                                const SizedBox(
                                  height: 10,
                                ),
                                Row(
                                  children: [
                                    AutoSizeText(
                                      'Слово не знаю, буду учить.',
                                      style: TextStyle(
                                          fontSize: 15,
                                          color: ColorsUtils.customBlackColor),
                                      maxLines: 1,
                                    ),
                                    const SizedBox(
                                      width: 10,
                                    ),
                                    Icon(
                                      Icons.school,
                                      color: ColorsUtils.customYellowColor,
                                    ),
                                  ],
                                ),
                              ],
                            ),
                          ],
                        )),
                  )
                ],
              ),
            ),
            pinned: false,
            snap: false,
            floating: true,
          ),
          SliverList(
            key: const ValueKey('usw_a2'),
            delegate: SliverChildBuilderDelegate(
              (BuildContext context, int index) {
                return Container(
                  padding: const EdgeInsets.symmetric(horizontal: 8),
                  color: index.isOdd
                      ? const Color.fromRGBO(254, 254, 250, .1)
                      : const Color.fromRGBO(190, 190, 247, .1),
                  child: WordSelectStateItem(
                    dictWord: _words[index],
                    isLast: index == _words.length - 1,
                    onChangeItemState: (state) {
                      var word = _words[index];
                      setState(() {
                        if (state == WordItemState.neutral.index) {
                          word.selected = false;
                          word.grade = 0;
                        } else if (state == WordItemState.unknown.index) {
                          word.selected = true;
                          word.grade = 0;
                        } else if (state == WordItemState.knoun.index) {
                          word.selected = false;
                          word.grade = 5;
                        }
                      });
                    },
                  ),
                );
              },
              childCount: _words.length,
            ),
          ),
        ],
      ),
      floatingActionButton: FloatingActionButton.extended(
        heroTag: 'h02',
        onPressed: () async {
          await save(1);
        },
        icon: const Icon(Icons.next_plan_outlined),
        label: const Text('Дальше'),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    debugPrint('!!!!!!! UserSettingsFirstScreen');
    return _isLoading
        ? _scfLoading()
        : _appSettings.nativeLanguage == null
            ? _scfSelectNativeLanguage()
            : _appSettings.learnLanguage == null
                ? _scfSelectLearnLanguage()
                : languageLevelWizardStep == null
                    ? _scfSelectLanguageLevel()
                    : _scfSelectLanguageLevelWizard();
  }
}
