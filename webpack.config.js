const path = require("path");
const webpack = require("webpack");
const fableUtils = require("fable-utils");
const HtmlWebpackPlugin = require('html-webpack-plugin');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');


function resolve(filePath) {
  return path.join(__dirname, filePath)
}

var babelOptions = fableUtils.resolveBabelOptions({
  presets: [
    ["env", {
      "targets": {
        "browsers": ["last 2 versions"]
      },
      "modules": false
    }],
  ],
});

var commonPlugins = [
  new HtmlWebpackPlugin({
    filename: resolve('./output/index.html'),
    template: resolve('./src/index.html')
  })
];

var isProduction = process.argv.indexOf("-p") >= 0;
console.log("Bundling for " + (isProduction ? "production" : "development") + "...");


module.exports = (env, argv) => ({
  mode: isProduction ? "production" : "development",
  devtool: isProduction ? false : 'source-map',
  entry: resolve('./src/Parsec.fsproj'),
  output: {
    path: resolve('./output'),
    filename: isProduction ? '[name].[hash].js' : '[name].js'
  },
  optimization: {
    splitChunks: {
      cacheGroups: {
        commons: {
          test: /[\\/]node_modules[\\/]/,
          name: 'vendor',
          chunks: 'all'
        },
        fable: {
          test: /[\\/]fable-core[\\/]/,
          name: 'fable',
          chunks: 'all'
        }
      }
    }
  },
  plugins: isProduction ?
    commonPlugins.concat([
      new ExtractTextPlugin('style.[contenthash].css'),
      new CopyWebpackPlugin([
        { from: './src' }
      ]),
      // ensure that we get a production build of any dependencies
      // this is primarily for React, where this removes 179KB from the bundle
      new webpack.DefinePlugin({
        'process.env.NODE_ENV': '"production"'
      })
    ])
    : commonPlugins.concat([
      new webpack.HotModuleReplacementPlugin(),
      new webpack.NamedModulesPlugin()
    ]),
  resolve: {
    modules: [
      "node_modules", resolve("./node_modules/")
    ]
  },
  devServer: {
    //contentBase: resolve('./public/'),
    publicPath: "/",
    port: 8080,
    hot: true,
    inline: true
  },

  module: {
    rules: [
      {
        test: /\.fs(x|proj)?$/,
        use: {
          loader: "fable-loader",
          options: {
            babel: babelOptions,
            define: isProduction ? [] : ["DEBUG"],
            extra: { optimizeWatch: true }
          }
        }
      },
      {
        test: /\.js$/,
        exclude: /node_modules/,
        use: {
          loader: 'babel-loader',
          options: babelOptions
        },
      },
      {
        test: /\.s(a|c)ss$/,
        use: isProduction ?
          ExtractTextPlugin.extract({
            fallback: 'style-loader',
            //resolve-url-loader may be chained before sass-loader if necessary
            use: ['css-loader', 'sass-loader']
          })
          : ["style-loader", "css-loader", "sass-loader"]
      },
      {
        test: /\.css$/,
        use: ['style-loader', 'css-loader']
      },
      {
        test: /\.(png|jpg|jpeg|gif|svg|woff|woff2|ttf|eot)(\?.*$|$)/,
        use: ["file-loader"]
      }
    ]
  }
});
