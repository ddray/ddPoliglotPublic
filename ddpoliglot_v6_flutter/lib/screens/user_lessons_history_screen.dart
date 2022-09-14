import 'package:ddpoliglot_v6_flutter/models/data/user_lesson.dart';
import 'package:ddpoliglot_v6_flutter/providers/database/database_provider.dart';
import 'package:ddpoliglot_v6_flutter/providers/playlist_provider.dart/play_list_provider.dart';
import 'package:ddpoliglot_v6_flutter/screens/user_lessons_results_screen.dart';
import 'package:ddpoliglot_v6_flutter/utils/colors_utils.dart';
import 'package:ddpoliglot_v6_flutter/utils/http_utils.dart';
import 'package:ddpoliglot_v6_flutter/utils/play_list_utils.dart';
import 'package:ddpoliglot_v6_flutter/utils/utils.dart';
import 'package:ddpoliglot_v6_flutter/widgets/grade_widget.dart';
import 'package:ddpoliglot_v6_flutter/widgets/loading_generate_lesson_widget.dart';
import 'package:ddpoliglot_v6_flutter/widgets/navigation_drawer_widget.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

class UserLessonsHistoryScreen extends StatefulWidget {
  const UserLessonsHistoryScreen({Key? key}) : super(key: key);
  static const routeName = '/user_lessons_history';

  @override
  UserLessonsHistoryScreenState createState() =>
      UserLessonsHistoryScreenState();
}

class UserLessonsHistoryScreenState extends State<UserLessonsHistoryScreen> {
  List<UserLesson> _userLessons = [];
  List<Item> _lessonPanels = [];
  // bool _lessonPanelExpanded = true;
  bool _isLoading = false;
  bool _isInit = false;
  int _totalSeconds = 0;
  int _learnSeconds = 0;
  LoadingData _loadingData = LoadingData();
  bool _isOnLine = false;

  @override
  void didChangeDependencies() {
    if (!_isInit) {
      _isInit = true;
      _isLoading = true;
      final databaseProvider =
          Provider.of<DatabaseProvider>(context, listen: false);
      final args = (ModalRoute.of(context)!.settings.arguments as int?) ?? 0;

      if ((databaseProvider.state.userLessons ?? []).isEmpty) {
        if (args == 0) {
          // no lessons, skip page
          Future.delayed(const Duration(microseconds: 10), () {
            final playListProvider =
                Provider.of<PlayListProvider>(context, listen: false);
            playListProvider
                .setPreparationStep(PreparationStep.configureLesson.index);
          });
        } else {}
      } else {
        HttpUtils.isOnLine().then((isOnLine) {
          setState(() {
            _isOnLine = isOnLine;
          });
        });

        Future.delayed(const Duration(microseconds: 10), () {
          setState(() {
            _userLessons = databaseProvider.state.userLessons!;
            _userLessons.sort((a, b) => b.num.compareTo(a.num));
            for (var userLesson in _userLessons) {
              _totalSeconds += userLesson.totalSeconds;
              _learnSeconds += userLesson.learnSeconds;
            }

            _lessonPanels = _userLessons.map<Item>((e) {
              return Item(
                  headerValue: 'Урок ${e.num}',
                  userLesson: e,
                  isExpanded: false); //(index++ == 0));
            }).toList();
            _isLoading = false;
          });
        });
      }
    }

    super.didChangeDependencies();
  }

  // Future<void> setLessonAsCurrent(UserLesson userLesson) async {
  //   setState(() {
  //     _isLoading = true;
  //   });
  //   final playListProvider =
  //       Provider.of<PlayListProvider>(context, listen: false);
  //   await playListProvider.finishLessonIfExists();
  //   await playListProvider.setWordsAndGeneratePlayLists(
  //       userLesson.userLessonWords
  //           .where((x) => x.wordType == 0)
  //           .map((e) => e.dictWord as DictWord)
  //           .toList(),
  //       userLesson.userLessonWords
  //           .where((x) => x.wordType == 1)
  //           .map((e) => e.dictWord as DictWord)
  //           .toList(),
  //       userLesson.num,
  //       setLoading);
  // }

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

  @override
  Widget build(BuildContext context) {
    final args = (ModalRoute.of(context)!.settings.arguments as int?) ?? 0;
    final playListProvider =
        Provider.of<PlayListProvider>(context, listen: false);
    debugPrint('UserLessonsHistoryScreen!!!!');

    return Scaffold(
      appBar: AppBar(
        title: const Text('Мои уроки'),
      ),
      drawer: args == 0
          ? NavigationDrawer(
              pageType: Pages.lessonsPage.index,
            )
          : null,
      floatingActionButton: (args >= 0 && _isOnLine)
          ? FloatingActionButton.extended(
              onPressed: () async {
                final isOnLine = await HttpUtils.isOnLine();
                if (isOnLine) {
                  playListProvider.finishLessonAndPlayOther();
                  if (args == 1) {
                    if (!mounted) return;
                    Navigator.of(context).pop();
                  }
                } else {
                  if (!mounted) return;
                  Utils.showErrorDialog(
                      'Ошибка подключения, проверьте интернет', context);
                }
              },
              icon: const Icon(Icons.add),
              label: const Text('Новый'),
            )
          : null,
      body: SingleChildScrollView(
        child: _isLoading
            ? LoadingGenerateLessonWidget(loadingData: _loadingData)
            : Column(
                mainAxisAlignment: MainAxisAlignment.start,
                children: [
                  Card(
                    elevation: 2,
                    margin: const EdgeInsets.all(5),
                    child: Column(
                      children: [
                        ResultLabelWidget(
                            label: 'Создано уроков:',
                            text: _userLessons.length.toString()),
                        ResultLabelWidget(
                            label: 'Общая длительность уроков:',
                            text: PlayListUtils.seconds2String(_totalSeconds)),
                        ResultLabelWidget(
                            label: 'Потрачено время на изучение:',
                            text: PlayListUtils.seconds2String(_learnSeconds)),
                      ],
                    ),
                  ),
                  const SizedBox(
                    height: 8,
                  ),
                  _buildLessonsPanels(),
                  const SizedBox(
                    height: 80,
                  )
                ],
              ),
      ),
    );
  }

  Widget _buildLessonsPanels() {
    final args = (ModalRoute.of(context)!.settings.arguments as int?) ?? 0;
    final playListProvider =
        Provider.of<PlayListProvider>(context, listen: false);
    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 5),
      child: ExpansionPanelList(
        expansionCallback: (int index, bool isExpanded) {
          setState(() {
            _lessonPanels[index].isExpanded = !isExpanded;
          });
        },
        children: _lessonPanels.map<ExpansionPanel>((Item item) {
          return ExpansionPanel(
            canTapOnHeader: true,
            headerBuilder: (BuildContext context, bool isExpanded) {
              return Container(
                color: item.userLesson.num.isOdd
                    ? const Color.fromRGBO(254, 254, 250, .1)
                    : const Color.fromRGBO(180, 180, 237, .1),
                child: ListTile(
                  leading: Text(item.headerValue),
                  title: LinearProgressIndicator(
                    value: (item.userLesson.learnSeconds <= 0 ||
                            item.userLesson.totalSeconds <= 0)
                        ? 0.01
                        : item.userLesson.learnSeconds /
                            item.userLesson.totalSeconds,
                  ),
                  subtitle: Text(
                      '${PlayListUtils.seconds2String(item.userLesson.learnSeconds)} / ${PlayListUtils.seconds2String(item.userLesson.totalSeconds)}'),
                  trailing: IconButton(
                    icon: Icon(
                      Icons.play_circle_outline,
                      color: ColorsUtils.customYellowColor,
                    ),
                    onPressed: () {
                      playListProvider.finishLessonAndPlayOther(
                          lessonNum: item.userLesson.num);
                      if (args == 1) {
                        Navigator.of(context).pop();
                      }
                    },
                  ),
                ),
              );
            },
            body: Column(
              children: [
                // SizedBox(
                //   width: double.infinity,
                //   child: Padding(
                //     padding: const EdgeInsets.fromLTRB(8, 8, 8, 8),
                //     child: Card(
                //       elevation: 10,
                //       child: Padding(
                //         padding: const EdgeInsets.all(8.0),
                //         child: Column(
                //           children: [
                //             Text(
                //                 '${DateFormat('d/M').add_Hm().format(item.userLesson.created)}   -   ${DateFormat('d/M').add_Hm().format(item.userLesson.finished)}'),
                //             Text(
                //                 'Duration: ${PlayListUtils.seconds2String(item.userLesson.totalSeconds)}'),
                //             Text(
                //                 'Пройденно: ${NumberFormat("##0.0", "en_US").format(item.userLesson.learnSeconds * 100 / item.userLesson.totalSeconds)}%'),
                //           ],
                //         ),
                //       ),
                //     ),
                //   ),
                // ),
                for (var lessonWord in item.userLesson.userLessonWords
                    .where((x) => x.wordType == 0))
                  Padding(
                      padding: const EdgeInsets.only(left: 4),
                      child: WordSelectRepeatItem(
                        dictWord: lessonWord.dictWord!,
                        onChangeItemState: null,
                        // (state) {
                        //   var word = lessonWord.dictWord!;
                        //   int grade = word.grade;
                        //   setState(() {
                        //     if (state == WordItemState.inProcess.index) {
                        //       word.selected = true;
                        //       if (word.grade == 0 || word.grade == 5) {
                        //         grade = 1;
                        //       }
                        //     } else {
                        //       word.selected = false;
                        //       grade = 5;
                        //     }
                        //   });

                        //   if (word.grade != grade) {
                        //     setState(() {
                        //       word.grade = grade;
                        //     });
                        //     WordUserService.setWordGrade(
                        //             word.grade,
                        //             word.wordID,
                        //             word.wordUserID,
                        //             Provider.of<DatabaseProvider>(context,
                        //                 listen: false))
                        //         .then((wordUser) {
                        //       setState(() {
                        //         word.loading = false;
                        //       });
                        //     });
                        //   }
                        // },
                      )

                      // DictWordItem(
                      //     dictWord: lessonWord.dictWord!,
                      //     canSelected: false,
                      //     onChangeSelection: null,
                      //     onChangeGrade: (grade) async {
                      //       var dictWord = lessonWord.dictWord!;
                      //       var oldGrade = dictWord.grade;
                      //       if (grade == oldGrade) return;
                      //       setState(() {
                      //         dictWord.grade = grade.toInt();
                      //         dictWord.loading = true;
                      //       });

                      //       WordUserService.setWordGrade(
                      //               dictWord.grade,
                      //               dictWord.wordID,
                      //               dictWord.wordUserID,
                      //               Provider.of<DatabaseProvider>(context,
                      //                   listen: false))
                      //           .then((wordUser) {
                      //         setState(() {
                      //           dictWord.wordUserID = wordUser.wordUserID;
                      //           dictWord.loading = false;
                      //         });
                      //       });
                      //     }),
                      )
              ],
            ),
            isExpanded: item.isExpanded,
          );
        }).toList(),
      ),
    );
  }
}

class Item {
  Item({
    required this.userLesson,
    required this.headerValue,
    this.isExpanded = false,
  });

  UserLesson userLesson;
  String headerValue;
  bool isExpanded;

  //(${e.created.day}.${e.created.month}.${e.created.year})
}
