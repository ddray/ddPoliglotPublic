import 'package:ddpoliglot_v6_flutter/models/user_settings_state_data.dart';
import 'package:ddpoliglot_v6_flutter/models/data/Language.dart';
import 'package:ddpoliglot_v6_flutter/providers/user_settings/user_settings_provider.dart';
import 'package:ddpoliglot_v6_flutter/services/language_service.dart';
import 'package:ddpoliglot_v6_flutter/services/user_language_level_service.dart';
import 'package:collection/collection.dart';
import 'package:ddpoliglot_v6_flutter/utils/http_utils.dart';
import 'package:ddpoliglot_v6_flutter/utils/utils.dart';

import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

class UserSettingsScreen extends StatefulWidget {
  const UserSettingsScreen({Key? key}) : super(key: key);
  static const routeName = '/user_settings';

  @override
  State<UserSettingsScreen> createState() => _UserSettingsScreenState();
}

class _UserSettingsScreenState extends State<UserSettingsScreen> {
  final _formKey = GlobalKey<FormState>();
  Language? _nativeLanguage;
  Language? _learnLanguage;
  bool _confSetWordGrade = true;
  bool _confClearWordGrade = true;
  int _wordsInLesson = 5;
  var _autovalidateMode = AutovalidateMode.disabled;
  late Future<List<Language>> _languages;
  var _changed = false;

  @override
  initState() {
    super.initState();
    _languages = LanguageService.getAll();
  }

  Future<void> save(BuildContext context,
      UserSettingsProvider userSettingsProvider, int? args) async {
    if (_formKey.currentState!.validate()) {
      // need to save appSettings for http equests first. we do it but without notification providers
      var appSetting = UserSettingsState(
          nativeLanguage: _nativeLanguage,
          learnLanguage: _learnLanguage,
          confClearWordGrade: _confClearWordGrade,
          confSetWordGrade: _confSetWordGrade,
          wordsInLesson: _wordsInLesson);
      await userSettingsProvider.update(appSetting, notify: false);
      var userLanguageLevel = await UserLanguageLevelService.get();
      await userSettingsProvider
          .update(appSetting.copyWith(userLanguageLevel: userLanguageLevel));
      // if ((args ?? 0) == 1) {
      //   Navigator.of(context).pop();
      // }
    } else {
      setState(() {
        _autovalidateMode = AutovalidateMode.always;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    final args = ModalRoute.of(context)!.settings.arguments as int?;
    final userSettingsProvider =
        Provider.of<UserSettingsProvider>(context, listen: true);
    debugPrint('UserSettingsScreen!!!!');
    return Scaffold(
      appBar: AppBar(
        title: const Text('User Settings'),
        actions: [
          IconButton(
            icon: const Icon(Icons.save),
            onPressed: _changed
                ? () async {
                    final isOnLine = await HttpUtils.isOnLine();
                    if (!mounted) return;
                    if (isOnLine) {
                      await save(context, userSettingsProvider, args);
                    } else {
                      Utils.showErrorDialog(
                          'Ошибка подключения, проверьте интернет', context);
                    }
                  }
                : null,
          ),
        ],
      ),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: _changed
            ? () async {
                final isOnLine = await HttpUtils.isOnLine();
                if (!mounted) return;
                if (isOnLine) {
                  await save(context, userSettingsProvider, args);
                } else {
                  Utils.showErrorDialog(
                      'Ошибка подключения, проверьте интернет', context);
                }
              }
            : null,
        icon: const Icon(Icons.save),
        label: const Text('Save'),
      ),
      body: Container(
          padding: const EdgeInsets.symmetric(horizontal: 2.0),
          child: FutureBuilder<List<Language>>(
            future: _languages,
            builder: (
              BuildContext context,
              AsyncSnapshot<List<Language>> snapshot,
            ) {
              if (snapshot.hasError) {
                throw Exception(
                    snapshot.error.toString() + snapshot.stackTrace.toString());
              } else if (snapshot.connectionState != ConnectionState.waiting) {
                if (_nativeLanguage == null) {
                  _nativeLanguage = snapshot.data!.firstWhereOrNull((x) =>
                          x.languageID ==
                          (userSettingsProvider
                                  .state.nativeLanguage?.languageID ??
                              2) // ru,
                      );
                  _learnLanguage = snapshot.data!.firstWhereOrNull((x) =>
                          x.languageID ==
                          (userSettingsProvider
                                  .state.learnLanguage?.languageID ??
                              1) // ru,,
                      );

                  _confClearWordGrade =
                      userSettingsProvider.state.confClearWordGrade;
                  _confSetWordGrade =
                      userSettingsProvider.state.confSetWordGrade;
                  _wordsInLesson = userSettingsProvider.state.wordsInLesson;
                }

                return
                    // userSettingsProvider.state.status ==
                    //         StateStatus.loading // ???????????? was prov.loaded
                    //     ? const Center(
                    //         child: CircularProgressIndicator(
                    //         color: Colors.green,
                    //       ))
                    //     :
                    Form(
                  key: _formKey,
                  autovalidateMode: _autovalidateMode,
                  child: ListView(
                    children: [
                      Container(
                        margin: const EdgeInsets.only(top: 10),
                        child: Card(
                          child: Column(
                            children: [
                              Padding(
                                padding:
                                    const EdgeInsets.symmetric(horizontal: 14),
                                child: Column(
                                  children: [
                                    Container(
                                      width: double.infinity,
                                      alignment: Alignment.centerLeft,
                                      child: const Text(
                                        'Мой родной язык',
                                        style: TextStyle(
                                          fontSize: 12,
                                        ),
                                      ),
                                    ),
                                    DropdownButtonFormField<Language>(
                                      value: _nativeLanguage,
                                      hint: const Text(
                                        'Select native language',
                                      ),
                                      onChanged: (_) {},
                                      //  (value) => setState(() {
                                      //   _changed = true;
                                      //   _nativeLanguage = value;
                                      // }),
                                      validator: (value) {
                                        if (value == null) {
                                          return 'field required';
                                        }

                                        if (value.code ==
                                            _learnLanguage?.code) {
                                          return 'should be different';
                                        }

                                        return null;
                                      },
                                      items: snapshot.data!
                                          .map<DropdownMenuItem<Language>>(
                                              (Language value) {
                                        return DropdownMenuItem<Language>(
                                          value: value,
                                          child: Text(value.codeFull,
                                              style: const TextStyle(
                                                  fontWeight: FontWeight.bold)),
                                        );
                                      }).toList(),
                                    ),
                                  ],
                                ),
                              ),
                              Padding(
                                padding:
                                    const EdgeInsets.symmetric(horizontal: 14),
                                child: Column(
                                  children: [
                                    Container(
                                      width: double.infinity,
                                      alignment: Alignment.centerLeft,
                                      child: const Text(
                                        'Я учу язык',
                                        style: TextStyle(
                                          fontSize: 12,
                                        ),
                                      ),
                                    ),
                                    DropdownButtonFormField<Language>(
                                      value: _learnLanguage,
                                      hint: const Text(
                                        'Select language to learn',
                                      ),
                                      onChanged: (_) {},

                                      // (value) => setState(() {
                                      //   _learnLanguage = value;
                                      //   _changed = true;
                                      // }),
                                      validator: (value) {
                                        if (value == null) {
                                          return 'field required';
                                        }

                                        if (value.code ==
                                            _nativeLanguage?.code) {
                                          return 'should be different';
                                        }

                                        return null;
                                      },
                                      items: snapshot.data!
                                          .map<DropdownMenuItem<Language>>(
                                              (Language value) {
                                        return DropdownMenuItem<Language>(
                                          value: value,
                                          child: Text(value.codeFull,
                                              style: const TextStyle(
                                                  fontWeight: FontWeight.bold)),
                                        );
                                      }).toList(),
                                    ),
                                  ],
                                ),
                              ),
                              const SizedBox(
                                height: 10,
                              )
                            ],
                          ),
                        ),
                      ),
                      Card(
                        child: Column(
                          children: [
                            ListTile(
                              title: const Text(
                                  'Подтверждать перевод слова в изученные'),
                              trailing: Switch(
                                value: _confSetWordGrade,
                                onChanged: (value) {
                                  setState(() {
                                    _confSetWordGrade = value;
                                    _changed = true;
                                  });
                                },
                              ),
                            ),
                            ListTile(
                              title: const Text(
                                  'Подтверждать перевод слова в не изученные'),
                              trailing: Switch(
                                value: _confClearWordGrade,
                                onChanged: (value) {
                                  setState(() {
                                    _confClearWordGrade = value;
                                    _changed = true;
                                  });
                                },
                              ),
                            ),
                            // ListTile(
                            //   title:
                            //       const Text('Количество слов в уроке'),
                            //   trailing: InputNumberWidget(
                            //       value: _wordsInLesson,
                            //       onPlus: () {
                            //         setState(() {
                            //           _wordsInLesson++;
                            //           _changed = true;
                            //         });
                            //       },
                            //       onMinus: () {
                            //         setState(() {
                            //           _wordsInLesson--;
                            //           _changed = true;
                            //         });
                            //       }),
                            // ),
                          ],
                        ),
                      ),
                    ],
                  ),
                );
              } else {
                return const Center(
                  child: CircularProgressIndicator(
                    color: Colors.green,
                  ),
                );
              }
            },
          )),
    );
  }
}
