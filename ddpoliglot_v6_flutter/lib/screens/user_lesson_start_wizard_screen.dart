import 'package:ddpoliglot_v6_flutter/exeptions/custom_exception.dart';
import 'package:ddpoliglot_v6_flutter/models/data/Log.dart';
import 'package:ddpoliglot_v6_flutter/models/dict_word.dart';
import 'package:ddpoliglot_v6_flutter/models/player_classes.dart';
import 'package:ddpoliglot_v6_flutter/providers/database/database_provider.dart';
import 'package:ddpoliglot_v6_flutter/providers/playlist_provider.dart/play_list_provider.dart';
import 'package:ddpoliglot_v6_flutter/services/log_service.dart';
import 'package:ddpoliglot_v6_flutter/services/word_user_service.dart';
import 'package:ddpoliglot_v6_flutter/utils/colors_utils.dart';
import 'package:ddpoliglot_v6_flutter/utils/http_utils.dart';
import 'package:ddpoliglot_v6_flutter/utils/utils.dart';
import 'package:ddpoliglot_v6_flutter/widgets/grade_widget.dart';
import 'package:ddpoliglot_v6_flutter/widgets/loading_generate_lesson_widget.dart';
import 'package:ddpoliglot_v6_flutter/widgets/navigation_drawer_widget.dart';
import 'package:ddpoliglot_v6_flutter/widgets/quest_button_widget.dart';
import 'package:ddpoliglot_v6_flutter/widgets/quest_screen_widget.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'dart:math' as math;
import 'package:auto_size_text/auto_size_text.dart';

import '../providers/user_settings/user_settings_provider.dart';

class UserLessonStartWizardScreen extends StatefulWidget {
  const UserLessonStartWizardScreen({Key? key, this.runLessonNum})
      : super(key: key);
  final int? runLessonNum;

  @override
  UserLessonStartWizardScreenState createState() =>
      UserLessonStartWizardScreenState();
}

class UserLessonStartWizardScreenState
    extends State<UserLessonStartWizardScreen> {
  List<DictWord>? _selectedWords;
  List<DictWord>? _words;
  List<DictWord>? _wordsForRepetition;
  int? _lessonType;

  int? _lessonNum;
  bool _isLoading = true;
  bool _filterSelectedOnly = false;
  bool _filterSelectedOnlyRep = false;

  bool _isLessonTypeSelected = false;
  bool _isLessonWordsSelected = false;
  bool _isLessonWordsSelectedSearch = false;
  bool _isLessonRepetitionWordsSelected = false;

  LoadingData _loadingData = LoadingData();
  bool _isInit = false;

  final TextEditingController _searchTextController = TextEditingController();
  String _filter = '';
  List<DictWord>? filteredWords;

  @override
  void dispose() {
    _searchTextController.dispose();
    super.dispose();
  }

  @override
  void initState() {
    super.initState();
    _searchTextController.addListener(() {
      debugPrint(_searchTextController.text);
      if (_searchTextController.text.length > 2) {
        setState(() {
          _filter = _searchTextController.text;
          filteredWords = (_words ?? [])
              .where((x) =>
                  x.textClear.toLowerCase().contains(_filter.toLowerCase()) ||
                  (x.translation ?? '')
                      .toLowerCase()
                      .contains(_filter.toLowerCase()))
              .take(10)
              .toList();
        });
      } else {
        setState(() {
          filteredWords = [];
        });
      }
    });
  }

  @override
  void didChangeDependencies() {
    if (!_isInit) {
      _isInit = true;
      _isLoading = true;
      final databaseProvider =
          Provider.of<DatabaseProvider>(context, listen: false);
      final playListProvider =
          Provider.of<PlayListProvider>(context, listen: false);

      if ((playListProvider.state.userLessonData?.runLessonNum ?? 0) > 0) {
        setState(() {
          // play lesson from hystory
          _lessonNum =
              (playListProvider.state.userLessonData?.runLessonNum ?? 0);
          playListProvider.state.userLessonData!.runLessonNum = 0;
          final lesson = databaseProvider.getLessonByNum(_lessonNum!);
          _lessonType = lesson.lessonType;

          _words = lesson.userLessonWords
              .map((e) => e.dictWord as DictWord)
              .toList();

          _selectedWords = lesson.userLessonWords
              .where((x) => x.wordType == 0)
              .map((e) => e.dictWord as DictWord)
              .toList();

          _wordsForRepetition = lesson.userLessonWords
              .where((x) => x.wordType != 0)
              .map((e) => e.dictWord as DictWord)
              .toList();

          for (var word in _selectedWords!) {
            word.selected = true;
          }

          for (var word in _wordsForRepetition!) {
            word.selected = true;
          }

          _isLessonTypeSelected = true;
          _isLessonWordsSelected = true;
          _isLessonWordsSelectedSearch = false;
          _isLessonRepetitionWordsSelected = true;
        });

        Future.delayed(const Duration(microseconds: 100), () {
          debugPrint('save delayed!!!');
          save(isNew: false).then((value) {
            final playListProvider =
                Provider.of<PlayListProvider>(context, listen: false);
            playListProvider.setLoading(false);
          });
        });
      } else {
        setState(() {
          // new lesson
          _lessonNum = databaseProvider.state.userLessons!.length + 1;
          _words = databaseProvider.getLessonCandidates();

          _wordsForRepetition =
              databaseProvider.getLessonRepetitionCandidates();
          for (var word in _wordsForRepetition!) {
            word.selected = true;
          }

          _selectedWords = [];
          _isLoading = false;
        });
      }
    }

    super.didChangeDependencies();
  }

  void setLoading(bool value,
      {String? label, String? msg, int? procent1, int? procent2}) {
    setState(() {
      _isLoading = value;
      if (!value) {
        _loadingData = LoadingData();
      } else {
        _loadingData = _loadingData.copyWith(
            message1: label,
            message2: msg,
            progress1: procent1,
            progress2: procent2);
      }
    });
  }

  _reOrderWords() {
    (_words ?? []).sort((a, b) {
      // if (a.fromSearch != b.fromSearch) {
      //   return a.fromSearch && !b.fromSearch ? -1 : 1;
      // }

      if (a.selected != b.selected) {
        return a.selected && !b.selected ? -1 : 1;
      }

      return a.rate.compareTo(b.rate);
    });
  }

  Widget _getSelectLessonType() {
    return QuestScreenWidget(
      question: 'Выберите тип урока.',
      answers: [
        QuestButtonWidget(
          'Краткий (10-20 минут).',
          () {
            _selectLessonType(LessonTypes.short.index);
          },
          subtitle: 'Интенсивное заучивание слов без фраз.',
        ),
        QuestButtonWidget(
          'Нормальный (40-60 минут).',
          () {
            _selectLessonType(LessonTypes.normal.index);
          },
          subtitle: 'Изучение слов и фраз в нормальном режиме',
        ),
        QuestButtonWidget(
          'Полный (1-2 часа).',
          () {
            _selectLessonType(LessonTypes.huge.index);
          },
          subtitle: 'Полный набор упражнений, идеален для фонового изучения.',
        ),
        QuestButtonWidget(
          'Short Test Words and phrases.',
          () {
            _selectLessonType(LessonTypes.testShort.index);
          },
          subtitle: 'Test words in short without slow variants.',
        ),
        QuestButtonWidget(
          'Full Test Words and phrases.',
          () {
            _selectLessonType(LessonTypes.testFull.index);
          },
          subtitle: 'Test words in short with slow variants.',
        ),
      ],
    );
  }

  _selectLessonType(int lessonType) async {
    final isOnLine = await HttpUtils.isOnLine();
    if (isOnLine) {
      setState(() {
        _isLessonTypeSelected = true;
        //_filterSelectedOnlyFirst = false;
        _filterSelectedOnly = false;
        _lessonType = lessonType;
      });
    } else {
      if (!mounted) return;
      Utils.showErrorDialog('Ошибка подключения, проверьте интернет', context);
    }
  }

  Future<int?> _showGradeConfirmationDialog(int type) async {
    return showDialog<int>(
      context: context,
      barrierDismissible: false,
      builder: (BuildContext context) {
        return Dialog(
          child: GradeConfirmationDialog(
            type: type,
          ),
        );
      },
    );
  }

  Future<bool> _changeGradeWithConfirmation(DictWord word, grade,
      UserSettingsProvider userSettingsProvider, bool autoFill) async {
    var oldGrade = word.grade;
    if (grade == oldGrade) return false;
    var needConfirmationType = (grade > 0 && oldGrade == 0)
        ? 1
        : (grade == 0 && oldGrade > 0)
            ? 2
            : 0;
    var confirmation = 1;
    var notAgain = false;
    if (needConfirmationType > 0) {
      if ((needConfirmationType == 1 &&
              userSettingsProvider.state.confSetWordGrade) ||
          (needConfirmationType == 2 &&
              (userSettingsProvider.state.confClearWordGrade))) {
        var res =
            (await _showGradeConfirmationDialog(needConfirmationType)) ?? 0;
        if (res > 10) {
          notAgain = true;
          res -= 10;
        }

        confirmation = res;
      }
    }

    if (confirmation == 0) return false;
    setState(() {
      word.loading = true;
    });

    if (await HttpUtils.isOnLine()) {
      if (!mounted) return false;
      WordUserService.setWordGrade(grade, word.wordID, word.wordUserID,
              Provider.of<DatabaseProvider>(context, listen: false))
          .then((wordUser) {
        setState(() {
          word.wordUserID = wordUser.wordUserID;
          word.loading = false;
        });
      });
    } else {
      if (!mounted) return false;
      Utils.showErrorDialog('Ошибка соединения, проверьте интернет', context);
      setState(() {
        _isLessonTypeSelected = false;
        _isLessonWordsSelected = false;
        _isLessonWordsSelectedSearch = false;
        _isLessonRepetitionWordsSelected = false;
      });
    }

    if (notAgain) {
      if (needConfirmationType == 1) {
        userSettingsProvider.update(
            userSettingsProvider.state.copyWith(confSetWordGrade: false));
      } else {
        userSettingsProvider.update(userSettingsProvider.state.copyWith(
          confClearWordGrade: false,
        ));
      }
    }

    return true;
  }

  Widget _getSelectLessonWords2() {
    final userSettingsProvider =
        Provider.of<UserSettingsProvider>(context, listen: true);
    final filteredWords =
        _words!.where((x) => x.selected || !_filterSelectedOnly).toList();

    return CustomScrollView(
      key: const ValueKey('uls_a3'),
      slivers: <Widget>[
        SliverAppBar(
          toolbarHeight: 150,
          backgroundColor: Colors.white,
          leading: const Text(''),
          leadingWidth: double.minPositive,
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
                  'Выберите 5-6 слов для урока.',
                  style: TextStyle(
                      fontSize: 20,
                      color: ColorsUtils.customBlackColor,
                      fontWeight: FontWeight.bold),
                  textAlign: TextAlign.center,
                  maxLines: 1,
                ),
                const SizedBox(
                  height: 20,
                ),
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Row(
                      children: [
                        Text(
                          _selectedWords!.length.toString(),
                          style: TextStyle(
                              fontSize: 25,
                              fontWeight: FontWeight.bold,
                              color: ColorsUtils.customYellow),
                        ),
                        const SizedBox(
                          width: 5,
                        ),
                        Switch(
                            value: _filterSelectedOnly,
                            onChanged: (value) {
                              setState(() {
                                _filterSelectedOnly = value;
                                _reOrderWords();
                              });
                            }),
                      ],
                    ),
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.end,
                      children: [
                        Row(
                          children: [
                            AutoSizeText(
                              'Слово знаю.',
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
                          height: 3,
                        ),
                        Row(
                          children: [
                            AutoSizeText(
                              'Слово учить.',
                              style: TextStyle(
                                  fontSize: 15,
                                  color: ColorsUtils.customBlackColor),
                              maxLines: 1,
                            ),
                            const SizedBox(
                              width: 10,
                            ),
                            Icon(
                              Icons.school_rounded,
                              color: ColorsUtils.customYellowColor,
                            ),
                          ],
                        ),
                      ],
                    ),
                  ],
                ),
              ],
            ),
          ),
          pinned: false,
          snap: false,
          floating: true,
        ),
        SliverList(
          key: const ValueKey('uls_a4'),
          delegate: SliverChildBuilderDelegate(
            (BuildContext context, int index) {
              return Container(
                  padding: const EdgeInsets.symmetric(horizontal: 5),
                  color: index.isOdd
                      ? const Color.fromRGBO(254, 254, 250, .1)
                      : const Color.fromRGBO(190, 190, 247, .1),
                  child: WordSelectStateItem(
                    dictWord: filteredWords[index],
                    isLast: index == filteredWords.length - 1,
                    onChangeItemState: (state) {
                      var word = filteredWords[index];
                      var grade = 0;
                      setState(() {
                        if (state == WordItemState.neutral.index) {
                          word.selected = false;
                          _selectedWords!
                              .removeWhere((x) => x.wordID == word.wordID);
                          grade = 0;
                        } else if (state == WordItemState.unknown.index) {
                          word.selected = true;
                          _selectedWords!.add(word);
                          grade = 0;
                        } else if (state == WordItemState.knoun.index) {
                          word.selected = false;
                          _selectedWords!
                              .removeWhere((x) => x.wordID == word.wordID);
                          grade = 5;
                        }
                      });

                      _changeGradeWithConfirmation(filteredWords[index], grade,
                              userSettingsProvider, false)
                          .then((value) {
                        if (value) {
                          setState(() {
                            var word = filteredWords[index];
                            word.grade = grade.toInt();
                            word.loading = false;
                          });
                        }
                      });
                    },
                  ));
            },
            childCount: filteredWords.length,
          ),
        ),
      ],
    );
  }

  Widget _getSelectRepetitionWords() {
    final userSettingsProvider =
        Provider.of<UserSettingsProvider>(context, listen: false);
    final filteredWordsRep = _wordsForRepetition!
        .where((x) => x.selected || !_filterSelectedOnlyRep)
        .toList();
    return CustomScrollView(
      key: const ValueKey('uls_a1'),
      slivers: <Widget>[
        SliverAppBar(
          toolbarHeight: 150,
          backgroundColor: Colors.white,
          leading: const Text(''),
          leadingWidth: double.minPositive,
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
                  'Повторим слова из предыдущих уроков.',
                  style: TextStyle(
                      fontSize: 20,
                      color: ColorsUtils.customBlackColor,
                      fontWeight: FontWeight.bold),
                  textAlign: TextAlign.center,
                  maxLines: 1,
                ),
                const SizedBox(
                  height: 20,
                ),
                Row(
                  mainAxisAlignment: MainAxisAlignment.end,
                  children: [
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.end,
                      children: [
                        Row(
                          mainAxisAlignment: MainAxisAlignment.end,
                          children: [
                            AutoSizeText(
                              'Слово знаю, повторять не надо.',
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
                          height: 3,
                        ),
                        Row(
                          mainAxisAlignment: MainAxisAlignment.end,
                          children: [
                            AutoSizeText(
                              'Слово учу, надо повторить.',
                              style: TextStyle(
                                  fontSize: 15,
                                  color: ColorsUtils.customBlackColor),
                              maxLines: 1,
                            ),
                            const SizedBox(
                              width: 10,
                            ),
                            Icon(
                              Icons.school_rounded,
                              color: ColorsUtils.customYellowColor,
                            ),
                          ],
                        ),
                      ],
                    ),
                  ],
                ),
              ],
            ),
          ),
          pinned: false,
          snap: false,
          floating: true,
        ),
        SliverList(
          key: const ValueKey('uls_a2'),
          delegate: SliverChildBuilderDelegate(
            (BuildContext context, int index) {
              return Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 5,
                  ),
                  color: index.isOdd
                      ? const Color.fromRGBO(254, 254, 250, .1)
                      : const Color.fromRGBO(190, 190, 247, .1),
                  child: WordSelectRepeatItem(
                    dictWord: filteredWordsRep[index],
                    isLast: index == filteredWordsRep.length - 1,
                    onChangeItemState: (state) {
                      var word = filteredWordsRep[index];
                      int grade = word.grade;
                      setState(() {
                        if (state == WordItemState.inProcess.index) {
                          word.selected = true;
                          if (word.grade == 0 || word.grade == 5) {
                            grade = 1;
                          }
                        } else {
                          word.selected = false;
                          grade = 5;
                        }
                      });

                      _changeGradeWithConfirmation(filteredWordsRep[index],
                              grade, userSettingsProvider, false)
                          .then((value) {
                        if (value) {
                          setState(() {
                            var word = filteredWordsRep[index];
                            word.grade = grade.toInt();
                            word.loading = false;
                          });
                        }
                      });
                    },
                  ));
            },
            childCount: filteredWordsRep.length,
          ),
        ),
      ],
    );
  }

  Widget _getSelectLessonWordsSearch() {
    int cnt = 0;
    return ListView(
      children: (filteredWords ?? [])
          .map((e) => GestureDetector(
                onTap: () {
                  setState(() {
                    e.selected = true;
                    _selectedWords!.add(e);
                    _isLessonWordsSelectedSearch = false;
                    _searchTextController.clear();
                    _reOrderWords();
                  });
                },
                child: Container(
                  padding: const EdgeInsets.fromLTRB(8, 6, 8, 6),
                  decoration: BoxDecoration(
                    border: Border(
                      bottom:
                          BorderSide(width: 1.0, color: Colors.grey.shade100),
                    ),
                    color: (cnt++).isOdd
                        ? const Color.fromRGBO(254, 254, 250, .1)
                        : const Color.fromRGBO(190, 190, 247, .1),
                  ),
                  child: Row(
                    children: [
                      RateWidget(rate: e.rate),
                      WordTextWidget(
                          text: e.text,
                          translation: e.translation ?? '',
                          selected: e.selected),
                      const Icon(
                        Icons.check,
                        color: Colors.grey,
                      )
                    ],
                  ),
                ),
              ))
          .toList(),
    );
  }

  Future<void> save({bool isNew = true}) async {
    if (_selectedWords!.isNotEmpty) {
      setState(() {
        _isLoading = true;
      });

      final playListProvider =
          Provider.of<PlayListProvider>(context, listen: false);

      try {
        debugPrint('run setWordsAndGeneratePlayLists');
        await playListProvider.setWordsAndGeneratePlayLists(
            _selectedWords!,
            _wordsForRepetition!.where((x) => x.selected).toList(),
            _lessonNum!,
            _lessonType!,
            isNew,
            setLoading);
      } catch (e) {
        if (e is CustomException) {
          Utils.showErrorDialog(e.screenMessage, context);
        }

        debugPrint(
            '---------------------  not enouph files: exeption ${e.toString()}');
        try {
          LogService.add(Log(0, playListProvider.authState.userId.toString(),
              'mob err', 'from mob', e.toString(), 1, DateTime.now()));
        } catch (_) {}

        setState(() {
          _isLoading = false;
          Provider.of<PlayListProvider>(context, listen: false)
              .setPreparationStep(PreparationStep.showLessonsList.index);
        });
      }
    }
  }

  Widget? _getFloatingActionButton() {
    return (_words ?? []).isNotEmpty
        ? _isLoading
            ? null
            : !_isLessonTypeSelected
                ? null
                : !_isLessonWordsSelected
                    ? _isLessonWordsSelectedSearch
                        ? null
                        : Row(
                            mainAxisAlignment: MainAxisAlignment.end,
                            children: [
                              FloatingActionButton.extended(
                                heroTag: 'h01',
                                onPressed: () {
                                  setState(() {
                                    _isLessonTypeSelected = false;
                                  });
                                },
                                icon: Transform(
                                  alignment: Alignment.center,
                                  transform: Matrix4.rotationY(math.pi),
                                  child: const Icon(Icons.next_plan_outlined),
                                ),
                                label: const Text('Назад'),
                              ),
                              const SizedBox(
                                width: 10,
                              ),
                              ((_wordsForRepetition?.length ?? 0) > 0)
                                  ? FloatingActionButton.extended(
                                      heroTag: 'h02',
                                      onPressed: () async {
                                        if (await HttpUtils.isOnLine()) {
                                          setState(() {
                                            _isLessonWordsSelected = true;
                                          });
                                        } else {
                                          Utils.showErrorDialog(
                                              'Ошибка соединения, проверьте интернет',
                                              context);
                                          setState(() {
                                            _isLessonTypeSelected = false;
                                            _isLessonWordsSelected = false;
                                            _isLessonWordsSelectedSearch =
                                                false;
                                            _isLessonRepetitionWordsSelected =
                                                false;
                                          });
                                        }
                                      },
                                      icon:
                                          const Icon(Icons.next_plan_outlined),
                                      label: const Text('Дальше'),
                                    )
                                  : FloatingActionButton.extended(
                                      heroTag: 'h03',
                                      onPressed: () async {
                                        if (await HttpUtils.isOnLine()) {
                                          await save();
                                        } else {
                                          Utils.showErrorDialog(
                                              'Ошибка соединения, проверьте интернет',
                                              context);
                                          setState(() {
                                            _isLessonTypeSelected = false;
                                            _isLessonWordsSelected = false;
                                            _isLessonWordsSelectedSearch =
                                                false;
                                            _isLessonRepetitionWordsSelected =
                                                false;
                                          });
                                        }
                                      },
                                      icon:
                                          const Icon(Icons.next_plan_outlined),
                                      label: const Text('Создать'),
                                    ),
                            ],
                          )
                    : !_isLessonRepetitionWordsSelected
                        ? Row(
                            mainAxisAlignment: MainAxisAlignment.end,
                            children: [
                              FloatingActionButton.extended(
                                heroTag: 'h04',
                                onPressed: () {
                                  setState(() {
                                    _isLessonWordsSelected = false;
                                    //_filterSelectedOnlyFirst = true;
                                    _filterSelectedOnly = true;
                                    _reOrderWords();
                                  });
                                },
                                icon: Transform(
                                  alignment: Alignment.center,
                                  transform: Matrix4.rotationY(math.pi),
                                  child: const Icon(Icons.next_plan_outlined),
                                ),
                                label: const Text('Назад'),
                              ),
                              const SizedBox(
                                width: 10,
                              ),
                              FloatingActionButton.extended(
                                heroTag: 'h05',
                                onPressed: () async {
                                  if (await HttpUtils.isOnLine()) {
                                    await save();
                                  } else {
                                    Utils.showErrorDialog(
                                        'Ошибка соединения, проверьте интернет',
                                        context);
                                    setState(() {
                                      _isLessonTypeSelected = false;
                                      _isLessonWordsSelected = false;
                                      _isLessonWordsSelectedSearch = false;
                                      _isLessonRepetitionWordsSelected = false;
                                    });
                                  }
                                },
                                icon: const Icon(Icons.next_plan_outlined),
                                label: const Text('Создать'),
                              ),
                            ],
                          )
                        : null
        : null;
  }

  List<Widget>? _getActions() {
    return !_isLoading
        ? !_isLessonTypeSelected
            ? null
            : !_isLessonWordsSelected
                ? _isLessonWordsSelectedSearch
                    ? null
                    : [
                        IconButton(
                            onPressed: () {
                              setState(() {
                                _searchTextController.clear();
                                _isLessonWordsSelectedSearch = true;
                              });
                            },
                            icon: const Icon(Icons.search)),
                        PopupMenuButton(
                          icon: Icon(_filterSelectedOnly
                              ? Icons.check
                              : Icons.more_vert),
                          onSelected: (val) {
                            if (val == 0) {
                              setState(() {
                                _filterSelectedOnly = !_filterSelectedOnly;
                                _reOrderWords();
                              });
                            } else if (val == 1) {
                              Utils.showInputDialog("set range", context)
                                  .then((value) {
                                try {
                                  debugPrint('value: $value');
                                  final ar = value.toString().split('-');
                                  final startRate = int.parse(ar[0].trim());
                                  final endRate = int.parse(ar[1].trim());
                                  setState(() {
                                    for (var word in _words!) {
                                      if (!word.selected &&
                                          word.grade == 0 &&
                                          word.rate >= startRate &&
                                          word.rate <= endRate) {
                                        word.selected = true;
                                        _selectedWords!.add(word);
                                        _changeGradeWithConfirmation(
                                                word,
                                                0,
                                                Provider.of<
                                                        UserSettingsProvider>(
                                                    context,
                                                    listen: false),
                                                false)
                                            .then((value) {
                                          if (value) {
                                            setState(() {
                                              word.grade = 0;
                                              word.loading = false;
                                            });
                                          }
                                        });
                                      }
                                    }
                                  });
                                } catch (_) {}
                              });
                            } else if (val == 2) {
                              setState(() {
                                for (var word in _words!) {
                                  word.selected = false;
                                }

                                _selectedWords = [];
                              });
                            }
                          },
                          itemBuilder: (_) => [
                            PopupMenuItem(
                              value: 0,
                              child: Row(
                                children: [
                                  if (_filterSelectedOnly)
                                    const Icon(Icons.check),
                                  const Text('Selected only'),
                                ],
                              ),
                            ),
                            PopupMenuItem(
                              value: 1,
                              child: Row(
                                children: const [
                                  Text('Select range'),
                                ],
                              ),
                            ),
                            PopupMenuItem(
                              value: 2,
                              child: Row(
                                children: const [
                                  Text('Deselect All'),
                                ],
                              ),
                            ),
                          ],
                        ),
                      ]
                : !_isLessonRepetitionWordsSelected &&
                        ((_wordsForRepetition?.length ?? 0) > 0)
                    ? [
                        PopupMenuButton(
                          icon: Icon(_filterSelectedOnlyRep
                              ? Icons.check
                              : Icons.more_vert),
                          onSelected: (val) {
                            if (val == 0) {
                              setState(() {
                                _filterSelectedOnlyRep =
                                    !_filterSelectedOnlyRep;
                              });
                            } else if (val == 1) {
                              setState(() {
                                _filterSelectedOnlyRep = false;
                                for (var word in (_wordsForRepetition ?? [])) {
                                  word.selected = false;
                                }
                              });
                            }
                          },
                          itemBuilder: (_) => [
                            PopupMenuItem(
                              value: 0,
                              child: Row(
                                children: [
                                  if (_filterSelectedOnlyRep)
                                    const Icon(Icons.check),
                                  const Text('Selected only'),
                                ],
                              ),
                            ),
                            PopupMenuItem(
                              value: 1,
                              child: Row(
                                children: const [
                                  Text('Clear all selected'),
                                ],
                              ),
                            ),
                          ],
                        ),
                      ]
                    : null
        : null;
  }

  Widget? _getAppBarTitle() {
    return !_isLessonWordsSelectedSearch
        ? Text('Урок ${_lessonNum ?? ''}')
        : Container(
            width: double.infinity,
            height: 40,
            decoration: BoxDecoration(
                color: Colors.white, borderRadius: BorderRadius.circular(5)),
            child: Center(
              child: TextField(
                controller: _searchTextController,
                autofocus: true,
                decoration: InputDecoration(
                    prefixIcon: const Icon(Icons.search),
                    suffixIcon: IconButton(
                      icon: const Icon(Icons.clear),
                      onPressed: () {
                        _searchTextController.value = TextEditingValue.empty;
                      },
                    ),
                    hintText: 'Search...',
                    border: InputBorder.none),
              ),
            ),
          );
  }

  @override
  Widget build(BuildContext context) {
    debugPrint('!!!!!!! UserLessonStartWizardScreen');
    return Scaffold(
      appBar: AppBar(
          leading: _isLessonWordsSelectedSearch
              ? IconButton(
                  onPressed: () {
                    setState(() {
                      _isLessonWordsSelectedSearch = false;
                    });
                  },
                  icon: const Icon(Icons.arrow_back_ios_new))
              : null,
          title: _getAppBarTitle(),
          actions: _getActions()),
      drawer: !_isLessonWordsSelectedSearch
          ? NavigationDrawer(
              pageType: Pages.lessonWizardPage.index,
            )
          : null,
      floatingActionButton:
          (_selectedWords?.length ?? 0) > 0 ? _getFloatingActionButton() : null,
      body: !_isLoading
          ? !_isLessonTypeSelected
              ? _getSelectLessonType()
              : !_isLessonWordsSelected
                  ? !_isLessonWordsSelectedSearch
                      ? _getSelectLessonWords2()
                      : _getSelectLessonWordsSearch()
                  : !_isLessonRepetitionWordsSelected &&
                          ((_wordsForRepetition?.length ?? 0) > 0)
                      ? _getSelectRepetitionWords()
                      : LoadingGenerateLessonWidget(
                          loadingData: _loadingData,
                        )
          : LoadingGenerateLessonWidget(loadingData: _loadingData),
    );
  }
}
