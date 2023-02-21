/**
 * Represents a JSONStorableBool in the Scripter plugin
 */

export class BoolParamDeclaration {
    /**
     * @type {boolean}
     */
    val;

    /**
     * Called when the value is changed
     * @param {function(boolean): void} callback
     */
    onChange(callback) { }
}