 // rollup.config.js
 import { nodeResolve } from '@rollup/plugin-node-resolve';

 export default {
    input: 'index.js',
    output: [{
      file: 'dist/bundled.js',
      sourcemap: 'inline',
      globals: {
        jquery: '$'
      }
    }],
    external: ['jquery'],
    treeshake: false,
    plugins: [
      nodeResolve()
    ]
  }