import 'package:ddpoliglot_v6_flutter/providers/database/database_provider.dart';
import 'package:ddpoliglot_v6_flutter/widgets/loading_generate_lesson_widget.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../exeptions/custom_exception.dart';
import '../utils/utils.dart';

class RefreshDatabaseScreen extends StatefulWidget {
  RefreshDatabaseScreen({Key? key}) : super(key: key) {
    debugPrint('create RefreshDatabaseScreen');
  }

  @override
  RefreshDatabaseScreenState createState() => RefreshDatabaseScreenState();
}

class RefreshDatabaseScreenState extends State<RefreshDatabaseScreen> {
  bool _isInit = false;
  LoadingData _loadingData = LoadingData();
  @override
  void didChangeDependencies() {
    if (!_isInit) {
      _isInit = true;
      final databaseDataProvider =
          Provider.of<DatabaseProvider>(context, listen: false);
      var errorMessage = '';
      try {
        databaseDataProvider.tryToRestoreFromStore((loadingData) {
          debugPrint('set state RefreshDatabaseScreen ${loadingData.message1}');
          setState(() {
            _loadingData = loadingData;
            debugPrint(
                'finish set state RefreshDatabaseScreen ${loadingData.message1}');
          });
        }).catchError((e) {
          errorMessage = 'Упс..., что то произошло. Попробуйте еще раз';
          Utils.showErrorDialog(errorMessage, context);
          throw Exception(e.toString());
        });
      } on CustomException catch (error) {
        errorMessage = error.message;
        Utils.showErrorDialog(errorMessage, context);
      } catch (error) {
        errorMessage = 'Упс..., произошла ошибка. Попробуйте еще раз';
        Utils.showErrorDialog(errorMessage, context);
        rethrow;
      }
    }
    super.didChangeDependencies();
  }

  @override
  Widget build(BuildContext context) {
    debugPrint('build RefreshDatabaseScreen');
    // final databaseDataProvider =
    //     Provider.of<DatabaseProvider>(context, listen: false);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Загрузка данных...'),
      ),
      body: LoadingGenerateLessonWidget(
          loadingData: _loadingData //databaseDataProvider.loadingData
          ),
    );
  }
}
