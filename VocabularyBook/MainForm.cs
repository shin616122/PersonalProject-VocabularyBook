﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace VocabularyBook
{
    public partial class MainForm : Form
    {
        private List<RowData> RowDataList { get; set; }
        private int CurrentRowNumber { get; set; }
        public string FilePath { get; set; }

        public MainForm()
        {
            InitializeComponent();

            // 問題パネルを非表示
            pnlQA.Hide();
        }

        /// <summary>
        /// フォーム
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            // TODO 全問 or 印 の選択

            // 指定ディレクトリ（EXEと同じPath）にある.xlsxを取得
            string[] filePaths = Directory.GetFiles($"{Path.GetDirectoryName(Application.ExecutablePath)}", "*.xlsx");

            if(filePaths.Length == 1)
            {
                // xlsxパスを変数に保存する
                FilePath = filePaths[0];

                // パスラベルを更新
                lbFilepath.Text = $@"ファイルパス: {FilePath}";

                // 問題一覧の取得
                RowDataList = LoadData.LoadDataFromExcel(FilePath);
            }
            else　if(filePaths.Length > 1)
            {
                //　複数エクセを見つかった場合
                MessageBox.Show("姉御に連絡して！ 複数エクセルが見つかりました。",
                                "残念でした",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);

                Environment.Exit(1);
            }
            else if(filePaths.Length <= 0)
            {
                //　エクセを見つからなかった場合
                MessageBox.Show("姉御に連絡して！ エクセルがありません。",
                                "残念でした",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);

                Environment.Exit(1);
            }

            // 問題一覧のシャッフル
            RowDataList = ShuffleList(RowDataList);

            // ロードした問題のID
            CurrentRowNumber = RowDataList.First().RowNumber;

            // 一番目の問題をtbxQuestionにロードする
            tbxQuestion.Text = RowDataList.First().Question;

            // 一番目の印をchkShouldReviewにロードする
            chkShouldReview.Checked = Convert.ToBoolean(RowDataList.First().ShouldReview);

            // tbxAnswerを初期化
            tbxAnswer.Text = String.Empty;
        }

        /// <summary>
        /// 回答ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnswer_Click(object sender, EventArgs e)
        {
            try
            {
                // 答えをtbxAnswerにロードする
                tbxAnswer.Text = RowDataList.Where(row => row.RowNumber == CurrentRowNumber).Select(row => row.Answer).FirstOrDefault();
            }
            catch (Exception ex)
            {
                MessageBox.Show("姉御に連絡して！ エラー: " + ex.Message,
                                "残念でした",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                throw ex;
            }
        }

#region いらないやつ
        /// <summary>
        /// A
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbxQuestion_TextChanged(object sender, EventArgs e)
        {
            // いらないやつ後で消す
        }

        /// <summary>
        /// B
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbxAnswer_TextChanged(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// ファイルパスのラベル
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbFilepath_Click(object sender, EventArgs e)
        {

        }
        #endregion

        /// <summary>
        /// 次へボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                // TODO もっときれいな書き方があるはず
                // もしCurrentRowNumberが最後なら、リセットする
                if(CurrentRowNumber != RowDataList.Last().RowNumber)
                {
                    // 次のRowNumberを取得
                    CurrentRowNumber = RowDataList.SkipWhile(row => row.RowNumber != CurrentRowNumber).Skip(1).First().RowNumber;

                    // 次の問題をロードする
                    tbxQuestion.Text = RowDataList.Where(row => row.RowNumber == CurrentRowNumber).Select(row => row.Question).FirstOrDefault();

                    // チェックボックスを更新する
                    chkShouldReview.Checked = Convert.ToBoolean(RowDataList.Where(row => row.RowNumber == CurrentRowNumber).Select(row => row.ShouldReview).FirstOrDefault());
                }
                else
                {
                    // 最初の問題のIDを更新する
                    CurrentRowNumber = RowDataList.First().RowNumber;

                    // 最初の問題をロードする
                    tbxQuestion.Text = RowDataList.First().Question; ;

                    // 最初の印をロードする
                    chkShouldReview.Checked = Convert.ToBoolean(RowDataList.First().ShouldReview);
                }

                // tbxAnswerを初期化
                tbxAnswer.Text = String.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show("姉御に連絡して！ エラー: " + ex.Message,
                                "残念でした",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                throw ex;
            }
        }

        /// <summary>
        /// 戻るボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBack_Click(object sender, EventArgs e)
        {
            try
            {
                // TODO もっときれいな書き方があるはず
                // もしCurrentRowNumberが最後なら、リセットする
                if (CurrentRowNumber != RowDataList.First().RowNumber)
                {
                    // ロードした問題のIDを更新する
                    CurrentRowNumber = RowDataList.TakeWhile(row => row.RowNumber != CurrentRowNumber).Last().RowNumber;

                    // 前の問題をロードする
                    tbxQuestion.Text = RowDataList.Where(row => row.RowNumber == CurrentRowNumber).Select(row => row.Question).FirstOrDefault();

                    // チェックボックスを更新する
                    chkShouldReview.Checked = Convert.ToBoolean(RowDataList.Where(row => row.RowNumber == CurrentRowNumber).Select(row => row.ShouldReview).FirstOrDefault());
                }
                else
                {
                    // 最後の問題のIDを更新する
                    CurrentRowNumber = RowDataList.Last().RowNumber;

                    // 最後の問題をロードする
                    tbxQuestion.Text = RowDataList.Last().Question;

                    // 最後の印をロードする
                    chkShouldReview.Checked = Convert.ToBoolean(RowDataList.Last().ShouldReview);
                }

                // tbxAnswerを初期化
                tbxAnswer.Text = String.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show("姉御に連絡して！ エラー: " + ex.Message,
                                "残念でした",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                throw ex;
            }
        }

        /// <summary>
        /// 印チェック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkShouldReview_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                // ListのshouldViewを更新する
                var currentRow = RowDataList.Where(row => row.RowNumber == CurrentRowNumber).First();
                currentRow.ShouldReview = Convert.ToInt32(chkShouldReview.Checked);

                // エクセルを更新する
                UpdateData.UpdateShouldReview(CurrentRowNumber, chkShouldReview.Checked, FilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("姉御に連絡して！ エラー: " + ex.Message,
                               "残念でした",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Exclamation);
                throw;
            }
        }

        
        /// <summary>
        /// 開始ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, EventArgs e)
        {
            pnlStart.Visible = false;
            pnlQA.Visible = true;
        }

        /// <summary>
        /// 印の問題のみ表示する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkReviewOnly_CheckedChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// リストをシャフルする
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">リストデータ</param>
        /// <returns>シャルルしたリストデータ</returns>
        private List<T> ShuffleList<T>(List<T> list)
        {
            return list.OrderBy(a => Guid.NewGuid()).ToList();
        }
    }
}