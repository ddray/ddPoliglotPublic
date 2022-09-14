import 'package:ddpoliglot_v6_flutter/models/data/user_lesson.dart';
import 'package:ddpoliglot_v6_flutter/providers/database/database_provider.dart';
import 'package:ddpoliglot_v6_flutter/providers/playlist_provider.dart/play_list_provider.dart';
import 'package:ddpoliglot_v6_flutter/utils/colors_utils.dart';
import 'package:ddpoliglot_v6_flutter/widgets/navigation_drawer_widget.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:syncfusion_flutter_charts/charts.dart';

class UserLessonsResultsScreen extends StatefulWidget {
  const UserLessonsResultsScreen({Key? key}) : super(key: key);
  static const routeName = '/user_lessons_results';

  @override
  State<UserLessonsResultsScreen> createState() =>
      _UserLessonsResultsScreenState();
}

class _UserLessonsResultsScreenState extends State<UserLessonsResultsScreen> {
  late List<UserLesson> _userLessons;
  bool _isLoading = false;
  bool _isInit = false;

  @override
  Future<void> didChangeDependencies() async {
    if (!_isInit) {
      _isInit = true;
      _isLoading = true;
      final databaseProvider =
          Provider.of<DatabaseProvider>(context, listen: false);

      if ((databaseProvider.state.userLessons ?? []).isEmpty) {
        // no lessons, skip page
        Future.delayed(const Duration(microseconds: 10), () {
          final playListProvider =
              Provider.of<PlayListProvider>(context, listen: false);
          playListProvider
              .setPreparationStep(PreparationStep.configureLesson.index);
        });
      } else {
        Future.delayed(const Duration(microseconds: 10), () {
          setState(() {
            _userLessons = databaseProvider.state.userLessons!;
            _userLessons.sort((a, b) => b.num.compareTo(a.num));
            _isLoading = false;
          });
        });
      }
    }

    super.didChangeDependencies();
  }

  DoughnutSeries<ChartData, String> _createSampleData(
      double unknown, double learning, double finished) {
    final List<ChartData> chartData = [
      ChartData('Новые', unknown.toDouble(), Colors.grey[300]!),
      ChartData('Учу', learning.toDouble(), ColorsUtils.customYellowColor),
      ChartData('Выучил', finished.toDouble(), Colors.amber[800]!),
    ];

    return DoughnutSeries<ChartData, String>(
      dataSource: chartData,
      pointColorMapper: (ChartData data, _) => data.color,
      xValueMapper: (ChartData data, _) => data.x,
      yValueMapper: (ChartData data, _) => data.y,
      dataLabelSettings: const DataLabelSettings(
          isVisible: true, labelPosition: ChartDataLabelPosition.outside),
    );
  }

  @override
  Widget build(BuildContext context) {
    final args = (ModalRoute.of(context)?.settings.arguments as int?) ?? 0;

    final playListProvider =
        Provider.of<PlayListProvider>(context, listen: true);

    final unknown = (playListProvider.databaseState?.dictWords ?? [])
        .where((x) => x.grade == 0)
        .toList()
        .length
        .toDouble();
    final learning = (playListProvider.databaseState?.dictWords ?? [])
        .where((x) => x.grade > 0 && x.grade < 5)
        .toList()
        .length
        .toDouble();
    final finished = (playListProvider.databaseState?.dictWords ?? [])
        .where((x) => x.grade == 5)
        .toList()
        .length
        .toDouble();
    final double learnPr = (learning + finished + unknown) > 0
        ? (((learning + finished) * 100) / (learning + finished + unknown))
        : 0;
    return Scaffold(
      appBar: AppBar(
        title: const Text('Результаты'),
      ),
      drawer: args == 0
          ? NavigationDrawer(
              pageType: Pages.resultsPage.index,
            )
          : null,
      floatingActionButton: args == 0
          ? FloatingActionButton.extended(
              onPressed: () {
                playListProvider
                    .setPreparationStep(PreparationStep.showLessonsList.index);
              },
              icon: const Icon(Icons.next_plan_outlined),
              label: const Text('Дальше'),
            )
          : null,
      body: SingleChildScrollView(
        child: _isLoading
            ? const Center(
                child: CircularProgressIndicator(
                color: Colors.yellowAccent,
              ))
            : Column(
                mainAxisAlignment: MainAxisAlignment.start,
                children: [
                  Padding(
                    padding: const EdgeInsets.all(8.0),
                    child: Card(
                      child: SfCircularChart(
                        key: UniqueKey(),
                        title: ChartTitle(text: 'На сколько выучили словарь.'),
                        legend: Legend(
                          isVisible: true,
                          // position: LegendPosition.bottom,
                          legendItemBuilder:
                              (legendText, series, point, seriesIndex) {
                            return SizedBox(
                              height: 30,
                              width: 100,
                              child: Row(
                                children: [
                                  seriesIndex == 0
                                      ? Icon(
                                          Icons.school,
                                          color: Colors.grey[300],
                                        )
                                      : seriesIndex == 1
                                          ? Icon(
                                              Icons.school,
                                              color:
                                                  ColorsUtils.customYellowColor,
                                            )
                                          : Icon(
                                              Icons.sentiment_satisfied_alt,
                                              color: Colors.amber[800],
                                            ),
                                  const SizedBox(
                                    width: 5,
                                  ),
                                  Text(legendText),
                                ],
                              ),
                            );
                          },
                          // Border color and border width of legend
                          // borderColor: Colors.black,
                          // borderWidth: 2
                        ),
                        annotations: <CircularChartAnnotation>[
                          // CircularChartAnnotation(
                          //     // radius: '50%',
                          //     widget: Container(
                          //         child: PhysicalModel(
                          //             child: Container(),
                          //             shape: BoxShape.circle,
                          //             elevation: 10,
                          //             shadowColor: Colors.black,
                          //             color: const Color.fromRGBO(
                          //                 230, 230, 230, 1)))),
                          CircularChartAnnotation(
                              // radius: '50%',
                              widget: Text('${learnPr.toInt().toString()}%',
                                  style: const TextStyle(
                                      color: Color.fromRGBO(0, 0, 0, 0.5),
                                      fontSize: 25)))
                        ],
                        series: <CircularSeries>[
                          _createSampleData(unknown, learning, finished),
                        ],
                      ),
                    ),
                  ),
                  const SizedBox(
                    height: 80,
                  )
                ],
              ),
      ),
    );
  }
}

class ResultLabelWidget extends StatelessWidget {
  const ResultLabelWidget({
    Key? key,
    required this.label,
    required this.text,
  }) : super(key: key);

  final String label;
  final String text;

  @override
  Widget build(BuildContext context) {
    return ListTile(
      title: Text(label),
      trailing: Text(text),
    );
    // Container(
    //   padding: const EdgeInsets.all(5),
    //   child: Row(
    //     children: [
    //       Expanded(
    //           child: Align(
    //         alignment: Alignment.centerRight,
    //         child: Padding(
    //           padding: const EdgeInsets.fromLTRB(0, 1, 5, 1),
    //           child: Text(
    //             label,
    //             style: const TextStyle(fontSize: 14),
    //           ),
    //         ),
    //       )),
    //       Expanded(
    //         child: Text(text,
    //             style:
    //                 const TextStyle(fontSize: 15, fontWeight: FontWeight.bold)),
    //       )
    //     ],
    //   ),
    // );
  }
}

class ChartData {
  final String x;
  final double y;
  final Color color;
  ChartData(this.x, this.y, this.color);
}
